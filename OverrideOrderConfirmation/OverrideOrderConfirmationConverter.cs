using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using Bridgevine;
using static BusinessRulesMigrator.Helpers;

namespace BusinessRulesMigrator.OverrideOrderConfirmation
{
    internal class OverrideOrderConfirmationConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.OverrideOrderConfirmationRules().GroupBy(r => r.GetDriverKey());

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

                foreach (var rule in group.ToList())
                {
                    if (rule.value.IsBlank())
                    {
                        Console.WriteLine($"ERROR: No confirmation message defined. BusinessRuleID {rule.BusinessRuleID} Value: {rule.value}");
                        continue;
                    }
                    else if (rule.IsFollowUpMessageByResultCode() && rule.Condition.IsBlank())
                    {
                        Console.WriteLine($"ERROR: The condition column must be defined. BusinessRuleID {rule.BusinessRuleID} Value: {rule.Condition}");
                        continue;
                    }

                    string message = rule.value;
                    int priority = 0;

                    string resultCode = null;
                    string offerCode = rule.OfferCode.IsBlank() ? null : rule.OfferCode;

                    if (rule.IsFollowUpMessageByResultCode())
                    {
                        var pairs =
                            rule.Condition.GetList(true)
                            .Select(s => s.GetList(false, @"[=]"))
                            .Where(l => l.Count == 2);

                        if (!pairs.Any())
                        {
                            Console.WriteLine($"ERROR: The condition column's value is not valid, please check it. BusinessRuleID {rule.BusinessRuleID} Value: {rule.Condition}");
                            continue;
                        }

                        var elems = new Dictionary<string, string>();

                        foreach (var pair in pairs)
                        {
                            elems[pair[0].ToUpper()] = pair[1];
                        }

                        if (elems.TryGetValue("PRIORITY", out string p)) 
                            int.TryParse(p, out priority);

                        if (!elems.TryGetValue("RESULTCODE", out resultCode) || resultCode.IsBlank())
                            resultCode = null;
                    }

                    var item = data.FirstOrDefault(i => i.Message.SameAs(message) && i.Priority == priority);
                    if (item.IsNull())
                    {
                        item = new Item
                        {
                            Message = message,
                            Priority = priority,
                            Criteria = new ConfirmationSpec(),
                        };
                        data.Add(item);
                    }

                    item.Criteria.AddOfferCode(offerCode);
                    item.Criteria.AddResultCode(resultCode);
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                if (data.Any())
                {
                    data.ForEach(item => item.UpdateCriteria());

                    converted.Add(GenerateRuleSql(RuleType.OverrideOrderConfirmation, Operation.SubmitOrder, driver, data));
                }
            }

            return converted;
        }
    }
}
