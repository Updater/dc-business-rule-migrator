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

namespace BusinessRulesMigrator.ConditionalOffers
{
    internal class ConditionalOfferConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.ConditionalOfferRules().GroupBy(r => r.GetDriverKey());

            if (!groups.Any()) return converted;

            var dataByDriver = new Dictionary<DriverKey, List<Availability>>();

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
                    if (!rule.ProviderID.HasValue || rule.OfferCode.IsBlank() || rule.OfferCode.SameAs("NULL"))
                    {
                        continue;
                    }

                    var availability = data.FirstOrDefault(a =>
                        a.ConditionalOffers.Safe()
                        .Any(o => o.HasProviderConstraint(rule.ProviderID.Value.ToString())));

                    if (availability.IsNull())
                    {
                        availability = new Availability
                        {
                            ConditionalOffers = new List<OffersSpec>
                            {
                                new OffersSpec
                                {
                                    ByProvider = new ByIDs
                                    {
                                        Condition = "IfAll",
                                        IDs = new List<string>
                                        {
                                            rule.ProviderID.Value.ToString()
                                        }
                                    },
                                    ByCode = new ByCode
                                    {
                                        Condition = "IfAny",
                                        Codes = new List<string>
                                        {
                                            rule.OfferCode
                                        }
                                    }
                                }
                            }
                        };
                        data.Add(availability);
                    }
                    else
                    {
                        //should never not find a result, and the result should always have at least one code.
                        var spec = availability.ConditionalOffers.First(o => o.HasProviderConstraint(rule.ProviderID.Value.ToString()));
                        spec.AddOfferCode(rule.OfferCode);
                    }
                }
            }

            foreach (var (driver, data) in dataByDriver)
            {
                if (data.Any())
                    converted.Add(GenerateRuleSql(RuleType.ConditionalOfferAvailability, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }
    }
}
