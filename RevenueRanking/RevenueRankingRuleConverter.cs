using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Offers;
using BusinessRulesMigrator.Common.Extensions;
using Bridgevine;
using static BusinessRulesMigrator.Helpers;

namespace BusinessRulesMigrator.RevenueRanking
{
    internal class RevenueRankingRuleConverter
    {
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
                        Console.WriteLine($"ERROR: an non integer RevenueRanking value found in BusinessRuleID {rule.BusinessRuleID}. Value: {rule.value}");
                        continue;
                    }
                    else if (value == 0)
                    {
                        // skip 0 value rules, they are unnecessary
                        continue;
                    }
                    else if (value < 1 || value > 5)
                    {
                        Console.WriteLine($"ERROR: an out of bounds RevenueRanking value found in BusinessRuleID {rule.BusinessRuleID}. Value: {rule.value}");
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

                    // By Offer Code
                    if (rule.OfferCode.IsNotBlank() && !rule.OfferCode.SameAs("NULL"))
                    {
                        if (item.Offers.ByCode.IsNull())
                        {
                            item.Offers.ByCode = new ByCode
                            {
                                Condition = "IfAny",
                                Codes = new List<string>()
                            };
                        }

                        if (!item.Offers.ByCode.Codes.Any(code => code.SameAs(rule.OfferCode)))
                        {
                            item.Offers.ByCode.Codes.Add(rule.OfferCode);
                        }
                    }

                    // By Offer Category
                    if (rule.OfferTypeID.HasValue && Offer.OfferCategories.ContainsKey(rule.OfferTypeID.Value))
                    {
                        if (item.Offers.ByProducts.IsNull())
                        {
                            item.Offers.ByProducts = new ByProducts
                            {
                                Condition = "IfAny",
                                Specs = new List<ProductSpec>()
                            };
                        }

                        item.Offers.ByProducts.Specs.Add(new ProductSpec
                        {
                            Condition = "HasExactly",
                            Products = new List<Product> 
                            { 
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
                        if (item.Offers.ByProducts.IsNull())
                        {
                            item.Offers.ByProducts = new ByProducts
                            {
                                Condition = "IfAny",
                                Specs = new List<ProductSpec>()
                            };
                        }
                        var type = Offer.OfferTypes[rule.OfferSubTypeID.Value];
                        item.Offers.ByProducts.Specs.Add(new ProductSpec 
                        {
                            Condition = "HasExactly",
                            Products = new List<Product>
                            {
                                new Product
                                {
                                    Category = Offer.GetOfferCategoryForOfferType(type.Category),
                                    Type = type.Type
                                }
                            }
                        });
                    }
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                if (data.Any())
                    converted.Add(GenerateRuleSql(RuleType.OverrideOfferRevenueRanking, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }

        private string GetRevenueRanking(int revenueRanking) => revenueRanking switch
        {
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            _ => "One"
        };
    }
}
