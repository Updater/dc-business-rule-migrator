using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;

namespace BusinessRulesMigrator.ConditionalOffers
{
    class Availability
    {
        public List<OffersSpec> ConditionalOffers { get; set; }

        public Requirement Requirement { get; set; }  
    }

    class RequirementCondition
    {
        public OfferAvailabilitySpec Left { get; set; }

        public OfferAvailabilitySpec Right { get; set; }

        public string LogicalOperator { get; set; }
    }

    class Visibility
    {
        public bool Show { get; set; }

        public bool ConditionalOffersMustExist { get; set; }
    }

    class Requirement
    {
        public RequirementCondition Condition { get; set; }

        public Visibility Visibility { get; set; }
    }
}
