using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using Bridgevine;
using static BusinessRulesMigrator.Helpers;

namespace BusinessRulesMigrator.ConditionalProviderAvailability
{
    internal class ConditionalProviderAvailabilityConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.ConditionalProvidersRules().GroupBy(r => r.GetDriverKey());

            if (!groups.Any()) return converted;

            var dataByDriver = new Dictionary<DriverKey, List<Availability>>();

            int[] GetIDs(string input) =>
                input.GetList(false)
                .Select(i => (ok: int.TryParse(i, out int id), id: id))
                .Where(i => i.ok)
                .Select(i => i.id)
                .Distinct()
                .ToArray();

            foreach (var group in groups)
            {
                var driver = group.Key;

                if (!dataByDriver.TryGetValue(driver, out var data))
                {
                    data = new List<Availability>();
                    dataByDriver[driver] = data;
                }

                foreach (var rule in group.ToList())
                {
                    if (rule.value.IsBlank())
                    {
                        Console.WriteLine($"ERROR: The value column is not defined. BusinessRuleID {rule.BusinessRuleID}.");
                        continue;
                    }

                    var items =
                        rule.value.GetList(true, @"[$]")
                        .Select(elem =>
                        {
                            var parts = elem.GetList(false, @"[~]");

                            if (parts.Count != 2) return null;

                            var availability = new Availability
                            {
                                Preferred = GetIDs(parts[0]),
                                Disfavored = GetIDs(parts[1]),
                            };

                            return availability.IsValid() ? availability : null;
                        })
                        .Where(a => a.IsNotNull());

                    data.AddRange(items);
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                if (data.Any())
                    converted.Add(GenerateRuleSql(RuleType.ConditionalProviderAvailability, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }
    }
}
