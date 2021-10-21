using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using BusinessRulesMigrator.Common.Offers;
using Bridgevine;

namespace BusinessRulesMigrator.OverrideOrderConfirmation
{
    internal class OverrideOrderConfirmationConverter
    {
        public List<string> Convert(IEnumerable<OldBusinessRule> rules)
        {
            var converted = new List<string>();

            var groups = rules.OverrideOrderConfirmationRules().GroupBy(r => r.GetDriverKey());

            if (!groups.Any()) return converted;

            var dataByDriver = new Dictionary<DriverKey, List<Item>>();

            foreach (var group in groups)
            {
                var driver = group.Key;

                foreach (var rule in group.ToList())
                {
                }
            }

            return converted;
        }
    }
}
