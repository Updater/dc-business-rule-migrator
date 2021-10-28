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
        OverrideOfferRevenueRanking = 7,
        OverrideOfferOrderingMethod = 8,
        OverrideValidationGroup = 9,
        OverrideOrderConfirmation = 10,
        InjectCustomizations = 11,
        InjectDisclosureCustomizations = 12,
        AddChannelLineupToOffers = 13,
    }
}
