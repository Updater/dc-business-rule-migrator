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

namespace BusinessRulesMigrator.OrderingMethod
{
    internal class OrderingMethodRuleConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.OrderingMethodRules().GroupBy(r => r.GetDriverKey());

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
                    var values = rule.value.GetList(false, @"[~]");

                    if (!values.Any())
                    {
                        Console.WriteLine($"ERROR: Invalid rule value in OrderingMethod rule. BusinessRuleID: {rule.BusinessRuleID} Value: {rule.value}");
                        continue;
                    }

                    var orderingMethod = new OrderingMethodData 
                    {
                        Type = values[0],
                        ImageUrl = values.Count > 1 ? values[1] : null,
                        ExternalUrl = values.Count > 2 ? values[2] : null,
                        Instructions = values.Count > 3 ? values[3] : null,
                    };

                    var item = data.FirstOrDefault(i => i.OrderingMethod.SameAs(orderingMethod));
                    if (item.IsNull())
                    {
                        item = new Item
                        {
                            OrderingMethod = orderingMethod,
                            Offers = new OffersSpec(),
                        };
                        data.Add(item);
                    }

                    item.Offers.AddOfferCode(rule.OfferCode);
                    item.Offers.AddProductByCategory(rule.OfferTypeID);
                    item.Offers.AddProductByType(rule.OfferSubTypeID);
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                if (data.Any())
                    converted.Add(GenerateRuleSql(RuleType.OverrideOfferOrderingMethod, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }
    }
}
