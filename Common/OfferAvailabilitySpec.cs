using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessRulesMigrator.Common
{
    internal class OfferAvailabilitySpec
    {
        public string Condition { get; set; }

        public List<OffersSpec> Specs { get; set; }
    }
}
