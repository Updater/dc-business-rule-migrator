using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using Bridgevine;
using Bridgevine.Json;
using static BusinessRulesMigrator.Helpers;
using CsvHelper;
using BusinessRulesMigrator.Common.CsvHelper;

namespace BusinessRulesMigrator.OverrideValidationGroup
{
    internal class OverrideValidationGroupConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.OverrideValidationGroupRules().GroupBy(r => r.GetDriverKey());

            if (!groups.Any()) return converted;

            var dataByDriver = new Dictionary<DriverKey, List<Item>>();

            foreach (var group in groups)
            {
                var driver = group.Key;

                if (!dataByDriver.TryGetValue(driver, out var data))
                {
                    data = new List<Item>();
                    dataByDriver[driver] = data;
                }

                var customizations = new Dictionary<string, Dictionary<string, object>>();

                void Add(string custCode, string choiceCode, Dictionary<string, object> props)
                {
                    var cust = customizations.FirstOrDefault(kv => kv.Key.SameAs(custCode));

                    if (choiceCode.IsBlank())
                    {
                        if (cust.Key.IsBlank())
                        {
                            props["Code"] = custCode;
                            customizations[custCode] = props;
                        }
                        else
                        {
                            foreach (var prop in props)
                            {
                                if (!prop.Key.SameAs("Code"))
                                    cust.Value[prop.Key] = prop.Value;
                            }
                        }
                    }
                    else 
                    {
                        if (cust.Key.IsBlank())
                        {
                            props["Code"] = choiceCode;
                            var choices = new Dictionary<string, Dictionary<string, object>> 
                            {
                                { choiceCode, props },
                            };
                            customizations[custCode] = new Dictionary<string, object> 
                            {
                                { "Code", custCode },
                                { "Choices", choices },
                            };
                        }
                        else
                        {
                            var choices = cust.Value.ContainsKey("Choices") ? cust.Value["Choices"] as Dictionary<string, Dictionary<string, object>> : null;
                            if (choices.IsNull())
                            {
                                choices = new Dictionary<string, Dictionary<string, object>>();
                                cust.Value["Choices"] = choices;
                            }
                            var choice = choices.FirstOrDefault(kv => kv.Key.SameAs(choiceCode));

                            if (choice.Key.IsBlank())
                            {
                                props["Code"] = choiceCode;
                                choices[choiceCode] = props;
                            }
                            else
                            {
                                foreach (var prop in props)
                                {
                                    if (!prop.Key.SameAs("Code"))
                                        choice.Value[prop.Key] = prop.Value;
                                }
                            }
                        }
                    }
                }

                foreach (var rule in group.ToList())
                {
                    if (rule.value.IsBlank())
                    {
                        Console.WriteLine($"ERROR: The value column is not defined. BusinessRuleID {rule.BusinessRuleID} Value: {rule.value}");
                        continue;
                    }

                    if (rule.IsOverrideCustomization() || rule.IsOverrideChoice())
                    {
                        var pairs =
                            rule.value.GetList(false, @"(?<=])\s*;\s*")
                            .Where(elem => elem.IsNotBlank())
                            .Select(elem =>
                            {
                                elem = elem.Trim();
                                int start = elem.IndexOf("[");
                                int end = elem.LastIndexOf("]");

                                if (start <= 0 || end <= 0 || start > end) return null;

                                string name = elem.Substring(0, start).RemoveSpaces();
                                string value = elem.Substring(start + 1, end - start - 1).Trim();

                                return name.IsBlank() ? null : new string[] { name, value };
                            })
                            .Where(arr => arr.Safe().Count() == 2);

                        if (!pairs.Any()) continue;

                        var props = new Dictionary<string, object>();

                        foreach (var pair in pairs)
                        {
                            string name = pair[0];
                            string value = pair[1];

                            props[name] =
                                name.SameAs("PriceList")
                                ? (value.IsBlank() ? null : BvJson.Deserialize(Regex.Replace(value, "\"", "'")))
                                : value;
                        }

                        Add(rule.CustomizationCode, rule.ChoiceCode, props);
                    }
                    else if (rule.IsCustomizationPrepopulate())
                    {
                        foreach (var value in rule.value.GetList(true, @"[,]"))
                        {
                            bool usingSemicolon = Regex.IsMatch(value, "^.*;.*;.*;.*;.*$");
                            bool usingColon = !usingSemicolon && Regex.IsMatch(value, "^.*:.*:.*:.*:.*$");

                            if (usingSemicolon || usingColon)
                            {
                                string[] parts = value.Split(usingColon ? ":" : ";");

                                if (parts.Length != 5)
                                {
                                    Console.WriteLine($"ERROR: Invalid section in Prepopulate Customization. BusinessRuleID {rule.BusinessRuleID} Section: {value}");
                                    continue;
                                }

                                string custCode = parts[0];
                                string choiceCode = parts[1];
                                bool? isChoiceSelected = "selected".SameAs(parts[2]) ? true : ("deselected".SameAs(parts[2]) ? false : null);
                                bool? showCustomization = "show".SameAs(parts[3]) ? true : ("hide".SameAs(parts[3]) ? false : null);
                                bool? isChoiceVisible = "showchoice".SameAs(parts[4]) ? true : ("hidechoice".SameAs(parts[4]) ? false : null);

                                if (custCode.IsNotBlank())
                                {
                                    if (showCustomization.HasValue)
                                    {
                                        var props = new Dictionary<string, object>
                                        {
                                            { "Display", showCustomization.Value },
                                        };
                                        Add(custCode, null, props);
                                    }
                                    if (choiceCode.IsNotBlank())
                                    {
                                        var props = new Dictionary<string, object>();

                                        if (isChoiceSelected.HasValue)
                                            props["Selected"] = isChoiceSelected.Value;

                                        if (isChoiceVisible.HasValue)
                                            props["Display"] = isChoiceVisible.Value;

                                        Add(custCode, choiceCode, props);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"ERROR: Invalid section in Prepopulate Customization. BusinessRuleID {rule.BusinessRuleID} Section: {value}");
                                    continue;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"ERROR: Invalid section in Prepopulate Customization. BusinessRuleID {rule.BusinessRuleID} Section: {value}");
                                continue;
                            }
                        }
                    }
                }

                if (!customizations.Any()) continue;

                foreach (var cust in customizations.Where(c => c.Value.ContainsKey("Choices")))
                {
                    var choices = cust.Value["Choices"] as Dictionary<string, Dictionary<string, object>>;

                    cust.Value["Choices"] = choices.Select(kv => kv.Value).ToArray();
                }

                data.Add(new Item
                {
                    Customizations = customizations.Select(kv => kv.Value).ToArray(),
                });
            }

            using (var reader = new StreamReader(@"C:\BusinessRules\ValidationGroupOverrides.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.TypeConverterCache.AddConverter<int?>(new NullableInt32Converter());
                csv.Context.TypeConverterCache.AddConverter<string>(new NullableStringConverter());
                var vgGroups = csv.GetRecords<ValidationGroupOverrideRule>().GroupBy(r => r.GetDriverKey()).ToList();

                foreach (var group in vgGroups)
                {
                    var driver = group.Key;

                    if (!dataByDriver.TryGetValue(driver, out var data))
                    {
                        data = new List<Item>();
                        dataByDriver[driver] = data;
                    }

                    foreach (var rule in group.ToList())
                    {
                        if (rule.ValidationGroupCode.IsBlank())
                        {
                            Console.WriteLine($"ERROR: Validation Group Override rule is not valid, ValidationGroupCode column must not be null, empty or all whitespaces. ValidationGroupOverrideID {rule.ValidationGroupOverrideID}");
                            continue;
                        }
                        if (rule.Name.IsNull() && rule.Description.IsNull())
                        {
                            Console.WriteLine($"ERROR: Validation Group Override rule is not valid, Name and Description columns are both null. ValidationGroupOverrideID {rule.ValidationGroupOverrideID}");
                            continue;
                        }

                        data.Add(new Item 
                        {
                            Code = rule.ValidationGroupCode,
                            Name = rule.Name,
                            Description = rule.Description,
                        });
                    }
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                if (data.Any())
                {
                    converted.Add(GenerateRuleSql(RuleType.OverrideValidationGroup, Operation.GetOrderRequirements, driver, data));
                    converted.Add(GenerateRuleSql(RuleType.OverrideValidationGroup, Operation.ValidateOrder, driver, data));
                }
            }

            return converted;
        }
    }
}
