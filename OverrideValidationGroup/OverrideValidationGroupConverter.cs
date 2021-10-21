using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using Bridgevine;
using static BusinessRulesMigrator.Helpers;

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

                foreach (var rule in group.ToList())
                {
                    if (rule.value.IsBlank())
                    {
                        Console.WriteLine($"ERROR: The value column is not defined. BusinessRuleID {rule.BusinessRuleID} Value: {rule.value}");
                        continue;
                    }

                    //var item = data.FirstOrDefault(i => i.Message.SameAs(message) && i.Priority == priority);
                    //if (item.IsNull())
                    //{
                    //    item = new Item
                    //    {
                    //        Message = message,
                    //        Priority = priority,
                    //        Criteria = new ConfirmationSpec(),
                    //    };
                    //    data.Add(item);
                    //}

                    //item.Criteria.AddOfferCode(offerCode);
                    //item.Criteria.AddResultCode(resultCode);
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
