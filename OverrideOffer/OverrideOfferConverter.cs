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

namespace BusinessRulesMigrator.OverrideOffer
{
    internal class OverrideOfferConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.RevenueRankingRules().GroupBy(r => r.GetDriverKey());

            if (!groups.Any()) return converted;

            var dataByDriver = new Dictionary<DriverKey, OverrideOffer>();

            foreach (var group in groups)
            {
                var driver = group.Key;

                if (!dataByDriver.TryGetValue(driver, out var data))
                {
                    data = new OverrideOffer();
                    dataByDriver[driver] = data;
                }

                foreach (var rule in group.ToList())
                {
                    if (!rule.ProviderID.HasValue)
                    {
                        continue;
                    }

                    string fieldName = rule.Entity.SameAs(Extensions.Entity.Offer) ? rule.EntityAttribute : rule.Entity;

                    data.OverrideByCode(rule.ProviderID.Value, rule.OfferCode, fieldName, rule.value);

                    if (!rule.OfferCode.IsBlank())
                    {
                        if (data.ByCode.IsNull())
                        {
                            data.ByCode = new OfferData[] { new OfferData { Code = rule.OfferCode } };
                        }

                        var offer = data.ByCode.FirstOrDefault(od => od.Code.SameAs(rule.OfferCode));
                    }

                    
                }

            }

            return converted;
        }
    }
}
