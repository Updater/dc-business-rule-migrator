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
                bool isNew = true;

                if (newRules.ContainsKey(key))
                {
                    isNew = false;
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
                }

                var item = items.FirstOrDefault(i => string.Equals(i.RevenueRanking, revenueRanking));
                if (item is null)
                {
                    item = new Item
                    {
                        RevenueRanking = revenueRanking,
                        Offers = new OffersSpec()
                    };
                }

                items.Add(item);

                // By offer code
                if (!string.IsNullOrEmpty(rule.OfferCode) && !string.Equals(rule.OfferCode, "NULL", StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Offers.ByCode is null)
                    {
                        item.Offers.ByCode = new ByCode
                        {
                            Condition = "IfAny",
                            Codes = new List<string>()
                        };
                    }

                    if (!item.Offers.ByCode.Codes.Contains(rule.OfferCode))
                    {
                        item.Offers.ByCode.Codes.Add(rule.OfferCode);
                    }
                }

                // By Offer Category
                if (rule.OfferTypeID.HasValue && Offer.OfferCategories.ContainsKey(rule.OfferTypeID.Value))
                {
                    if (item.Offers.ByProducts is null)
                    {
                        item.Offers.ByProducts = new ByProducts
                        {
                            Condition = "IfAny",
                            Specs = new List<ProductSpec>()
                        };
                    }

                    item.Offers.ByProducts.Specs.Add(
                        new ProductSpec
                        {
                            Condition = "HasExactly",
                            Products = new List<Product>{ 
                                new Product 
                                { 
                                    Category = Offer.OfferCategories[rule.OfferTypeID.Value] 
                                } 
                            }
                        });
                }

                // By Offer Type
                if (rule.OfferSubTypeID.HasValue && Offer.OfferTypes.ContainsKey(rule.OfferSubTypeID.Value))
                {
                    if (item.Offers.ByProducts is null)
                    {
                        item.Offers.ByProducts = new ByProducts
                        {
                            Condition = "IfAny",
                            Specs = new List<ProductSpec>()
                        };
                    }
                    var type = Offer.OfferTypes[rule.OfferSubTypeID.Value];
                    item.Offers.ByProducts.Specs.Add(
                        new ProductSpec
                        {
                            Condition = "HasExactly",
                            Products = new List<Product>{
                                new Product
                                {
                                    Category = Offer.GetOfferCategoryForOfferType(type.Category),
                                    Type = type.Type
                                }
                            }
                        });
                }

                if (isNew)
                {
                    newRules.Add(key, items);
                }
                
            }

            foreach (var pair in newRules)
            {
                var sql = Helpers.GenerateRuleSql(pair.Key, RuleType.OverrideOfferRevenueRanking, 1, JsonSerializer.Serialize(pair.Value));

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
