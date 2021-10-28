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

namespace BusinessRulesMigrator.RemoveProviders
{
    internal class RemoveProvidersConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.RemoveProvidersRules().GroupBy(r => r.GetDriverKey());

            if (!groups.Any()) return converted;

            var dataByDriver = new Dictionary<DriverKey, List<int>>();

            IEnumerable<int> GetIDs(string input) =>
                input.GetList(true)
                .Select(i => (ok: int.TryParse(i, out int id), id: id))
                .Where(i => i.ok)
                .Select(i => i.id)
                .Distinct()
                .ToArray();

            foreach (var group in groups)
            {
                var driver = group.Key;
                driver.ProviderId = null;

                if (!dataByDriver.TryGetValue(driver, out var data))
                {
                    data = new List<int>();
                    dataByDriver[driver] = data;
                }

                foreach (var rule in group.ToList())
                {
                    var ids = GetIDs(rule.value).Except(data);

                    data.AddRange(ids);
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                converted.Add(GenerateRuleSql(RuleType.RemoveProviders, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }
    }
}
