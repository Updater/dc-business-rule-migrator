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

namespace BusinessRulesMigrator.RevenueRanking
{
    internal class RevenueRankingRuleConverter
    {
        private string GetRevenueRanking(int revenueRanking) => revenueRanking switch
        {
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            _ => "One"
        };

        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.RevenueRankingRules().GroupBy(r => r.GetDriverKey());

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
                    if (!int.TryParse(rule.value, out int value))
                    {
                        Console.WriteLine($"ERROR: A non integer RevenueRanking value found. BusinessRuleID {rule.BusinessRuleID} Value: {rule.value}");
                        continue;
                    }
                    else if (value == 0)
                    {
                        continue;
                    }
                    else if (value < 1 || value > 5)
                    {
                        Console.WriteLine($"ERROR: An out of bounds RevenueRanking value found. BusinessRuleID {rule.BusinessRuleID} Value: {rule.value}");
                        continue;
                    }

                    var revenueRanking = GetRevenueRanking(value);

                    var item = data.FirstOrDefault(i => i.RevenueRanking.SameAs(revenueRanking));
                    if (item.IsNull())
                    {
                        item = new Item
                        {
                            RevenueRanking = revenueRanking,
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
                    converted.Add(GenerateRuleSql(RuleType.OverrideOfferRevenueRanking, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }
    }
}
