using System;
using System.Collections.Generic;
using System.Text;
using BusinessRulesMigrator.Common;
using System.Linq;
using Bridgevine;

namespace BusinessRulesMigrator.ConditionalProviderAvailability
{
    class Availability
    {
        public int[] Preferred { get; set; }

        public int[] Disfavored { get; set; }

        public bool IsValid() => Preferred.Safe().Any() && Disfavored.Safe().Any();
    }
}
