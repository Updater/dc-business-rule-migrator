using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Offers;

namespace BusinessRulesMigrator.RevenueRanking
{
    internal class RevenueRankingRuleConverter
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

                int rr;
                if (!int.TryParse(rule.value, out rr))
                {
                    Console.WriteLine($"ERROR: an non integer RevenueRanking value found in BusinessRuleID {rule.BusinessRuleID}.  Value: {rule.value}");
                    continue;
                }
                else if (rr == 0)
                {
                    //skip 0 value rules, they are unnecessary
                    continue;
                }
                else if (rr < 1 || rr > 5)
                {
                    Console.WriteLine($"ERROR: an out of bounds RevenueRanking value found in BusinessRuleID {rule.BusinessRuleID}.  Value: {rule.value}");
                    continue;
                }
                
                var revenueRanking = GetRevenueRankingString(rr);
                var key = Helpers.GetKey(rule);
                List<Item> items = null;

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
                            RevenueRanking = revenueRanking,
                            Offers = new OffersSpec()
                        }
                    };

                    newRules.Add(key, items);
                }

                var item = items.FirstOrDefault(i => string.Equals(i.RevenueRanking, revenueRanking));
                if (item is null)
                {
                    item = new Item
                    {
                        RevenueRanking = revenueRanking,
                        Offers = new OffersSpec()
                    };

                    items.Add(item);
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

        

        private string GetRevenueRankingString(int revenueRanking)
        {
            switch (revenueRanking)
            {
                case 1:
                    return "One";
                case 2:
                    return "Two";
                case 3:
                    return "Three";
                case 4:
                    return "Four";
                case 5:
                    return "Five";
                default:
                    return "One";
            }
        }
    }
}
