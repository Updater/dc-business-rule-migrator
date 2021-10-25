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

            var groups = rules.OverrideOfferRules().GroupBy(r => r.GetDriverKey());

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

                    data.OverrideByCode(rule.ProviderID.Value, rule.OfferCode, fieldName, rule.value, rule.ActionTypeID.Value);
                    data.OverrideByProduct(rule.OfferTypeID, rule.OfferSubTypeID, fieldName, rule.value, rule.ActionTypeID.Value);
                }

            }

            foreach (var (driver, data) in dataByDriver)
            {
                if ((!data.ByCode.IsNull() && data.ByCode.Any()) || (!data.ByCode.IsNull() && data.ByProducts.Any()))
                    converted.Add(GenerateRuleSql(RuleType.OverrideOffer, Operation.GetOfferAvailability, driver, data));
            }

            return converted;
        }
    }
}
