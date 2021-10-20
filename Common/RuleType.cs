using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessRulesMigrator.Common
{
    public enum RuleType : int
    {
        RemoveProviders = 1,
        ConditionalProviderAvailability = 2,
        RemoveOffersGlobally = 3,
        RemoveOffersForProvider = 4,
        ConditionalOfferAvailability = 5,
        OverrideOffer = 6,
        OverrideOfferGroup = 7,
        OverrideOfferRevenueRanking = 8,
        OverrideOfferOrderingMethod = 9,
        OverrideValidationGroup = 10,
        OverrideOrderConfirmation = 11,
    }
}
