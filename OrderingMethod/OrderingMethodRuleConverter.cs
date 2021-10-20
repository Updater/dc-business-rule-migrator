using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Offers;

namespace BusinessRulesMigrator.OrderingMethod
{
    internal class OrderingMethodRuleConverter
    {
        public List<string> ConvertRules(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();
            Dictionary<DriverKey, List<Item>> newRules = new Dictionary<DriverKey, List<Item>>();

            foreach (var rule in rules)
            {
                //ProviderID required for this rule
                if (!rule.ProviderID.HasValue)
                {
                    continue;
                }

                string[] values = rule.value.Split('~');

                if (values.Length < 1)
                {
                    Console.WriteLine($"ERROR: Invalid rule value in OrderingMethod rule.  BusinessRuleID: {rule.BusinessRuleID}  Value: {rule.value}");
                    continue;
                }

                var orderingMethod = new OrderingMethodData();
                for (var i = 0; i < values.Length; i++)
                {
                    var value = values[i].Trim();

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    switch (i)
                    {
                        case 0:
                            orderingMethod.Type = value;
                            break;
                        case 1:
                            orderingMethod.ImageUrl = value;
                            break;
                        case 2:
                            orderingMethod.ExternalUrl = value;
                            break;
                        case 3:
                            orderingMethod.Instructions = value;
                            break;
                    }
                }

                var key = Helpers.GetKey(rule);
                List<Item> items;

                if (newRules.ContainsKey(key))
                {
                    items = newRules[key];
                }
                else
                {
                    items = new List<Item>
                        {
                            new Item
                            {
                                OrderingMethod = orderingMethod,
                                Offers = new OffersSpec()
                            }
                        };

                    newRules.Add(key, items);
                }

                var item = items.FirstOrDefault(i => i.OrderingMethod.Equals(orderingMethod));

                if (item is null)
                {
                    item = new Item
                    {
                        OrderingMethod = orderingMethod,
                        Offers = new OffersSpec()
                    };
                }

                Helpers.PopulateOfferSpecByCodeByProvider(rule, item.Offers);
            }

            foreach (var pair in newRules)
            {
                var sql = Helpers.GenerateRuleSql(pair.Key, RuleType.OverrideOfferRevenueRanking, 1, pair.Value);

                if (!string.IsNullOrEmpty(sql))
                {
                    converted.Add(sql);
                }
            }

            return converted;
        }
    }
}
