using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridgevine;

namespace BusinessRulesMigrator.Common.Extensions
{
    internal static class Extensions
    {
        public static class Entity
        {
            public const string Offer = "Offer";
            public const string OrderConfirmation = "OrderConfirmation";
            public const string Bonus = "Bonus";
            public const string Promotion = "Promotion";
            public const string Customization = "Customization";
            public const string Choice = "Choice";
            public const string Provider = "Provider";
            public const string Disclosure = "Disclosure";
        }

        public static class EntityAttribute
        { 
            public const string RevenueRanking = "RevenueRanking";
            public const string OrderingMethod = "OrderingMethod";
            public const string ProviderFollowUpMessage = "ProviderFollowUpMessage";
            public const string ProviderFollowUpMessageByResultCode = "ProviderFollowUpMessageByResultCode";
            public const string ShortHighlight1 = "ShortHighlight1";
            public const string ShortHighlight2 = "ShortHighlight2";
            public const string ShortHighlight3 = "ShortHighlight3";
            public const string ShortHighlight4 = "ShortHighlight4";
            public const string Disclaimer = "Disclaimer";
            public const string ProviderLogo = "ProviderLogo";
            public const string MediumName = "MediumName";
            public const string Name = "Name";
            public const string BenefitStatementLong = "BenefitStatementLong";
            public const string ProviderLogoLarge = "ProviderLogoLarge";
            public const string HardwarePrice = "HardwarePrice";
            public const string Highlight = "Highlight";
            public const string InstallPrice = "InstallPrice";
            public const string DepositPrice = "DepositPrice";
            public const string ShortTermPrice = "ShortTermPrice";
            public const string Prepopulate = "Prepopulate";
            public const string ConditionalOffers = "ConditionalOffers";
            public const string ConditionalProviders = "ConditionalProviders";
            public const string Customization = "Customization";
        }

        public static OldBusinessRule[] RevenueRankingRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.Entity.SameAs(Entity.Offer) && r.EntityAttribute.SameAs(EntityAttribute.RevenueRanking))
            .ToArray();

        public static OldBusinessRule[] OrderingMethodRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.Entity.SameAs(Entity.Offer) && r.EntityAttribute.SameAs(EntityAttribute.OrderingMethod))
            .ToArray();

        public static bool IsFollowUpMessage(this OldBusinessRule r) => r.EntityAttribute.SameAs(EntityAttribute.ProviderFollowUpMessage);

        public static bool IsFollowUpMessageByResultCode(this OldBusinessRule r) => r.EntityAttribute.SameAs(EntityAttribute.ProviderFollowUpMessageByResultCode);

        public static OldBusinessRule[] OverrideOrderConfirmationRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.Entity.SameAs(Entity.OrderConfirmation))
            .Where(r => r.IsFollowUpMessage() || r.IsFollowUpMessageByResultCode())
            .ToArray();

        public static OldBusinessRule[] OverrideOfferRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => 
                (
                    r.Entity.SameAs(Entity.Offer) && 
                    (
                    r.EntityAttribute.SameAs(EntityAttribute.ShortHighlight1) ||
                    r.EntityAttribute.SameAs(EntityAttribute.ShortHighlight2) ||
                    r.EntityAttribute.SameAs(EntityAttribute.ShortHighlight3) ||
                    r.EntityAttribute.SameAs(EntityAttribute.ShortHighlight4) ||
                    r.EntityAttribute.SameAs(EntityAttribute.Disclaimer) ||
                    r.EntityAttribute.SameAs(EntityAttribute.ProviderLogo) ||
                    r.EntityAttribute.SameAs(EntityAttribute.MediumName) ||
                    r.EntityAttribute.SameAs(EntityAttribute.Name) ||
                    r.EntityAttribute.SameAs(EntityAttribute.BenefitStatementLong) ||
                    r.EntityAttribute.SameAs(EntityAttribute.ProviderLogoLarge) ||
                    r.EntityAttribute.SameAs(EntityAttribute.HardwarePrice) ||
                    r.EntityAttribute.SameAs(EntityAttribute.Highlight) ||
                    r.EntityAttribute.SameAs(EntityAttribute.InstallPrice) ||
                    r.EntityAttribute.SameAs(EntityAttribute.DepositPrice) ||
                    r.EntityAttribute.SameAs(EntityAttribute.ShortTermPrice)
                    )
                ) ||
                r.Entity.SameAs(Entity.Bonus) ||
                r.Entity.SameAs(Entity.Promotion)
            )
            .ToArray();

        public static OldBusinessRule[] ConditionalOfferRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.Entity.SameAs(Entity.Offer) && r.EntityAttribute.SameAs(EntityAttribute.ConditionalOffers))
            .ToArray();

        public static bool IsCustomizationPrepopulate(this OldBusinessRule r) =>
            r.Entity.SameAs(Entity.Customization) && r.EntityAttribute.SameAs(EntityAttribute.Prepopulate);

        public static bool IsOverrideCustomization(this OldBusinessRule r) =>
            r.ActionTypeID == ActionType.Replace &&
            r.Entity.SameAs(Entity.Customization) && 
            r.EntityAttribute.IsNotBlank() &&
            r.EntityAttribute.SameAs(r.CustomizationCode) &&
            r.ChoiceCode.IsBlank();

        public static bool IsOverrideChoice(this OldBusinessRule r) =>
            r.ActionTypeID == ActionType.Replace &&
            r.Entity.SameAs(Entity.Choice) &&
            r.EntityAttribute.IsNotBlank() &&
            r.EntityAttribute.SameAs(r.ChoiceCode) &&
            r.CustomizationCode.IsNotBlank();

        public static OldBusinessRule[] OverrideValidationGroupRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.IsCustomizationPrepopulate() || r.IsOverrideCustomization() || r.IsOverrideChoice())
            .ToArray();

        public static OldBusinessRule[] ConditionalProvidersRules(this IEnumerable<OldBusinessRule> rules) =>
            rules.Where(r => r.Entity.SameAs(Entity.Provider) && r.EntityAttribute.SameAs(EntityAttribute.ConditionalProviders))
            .ToArray();

        public static OldBusinessRule[] InjectCustomizationsRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ActionTypeID == ActionType.Add)
            .Where(r => r.Entity.SameAs(Entity.Customization))
            .Where(r => r.EntityAttribute.IsBlank())
            .ToArray();

        public static OldBusinessRule[] InjectDisclosureCustomizationsRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ActionTypeID == ActionType.Add || r.ActionTypeID == ActionType.Delete)
            .Where(r => r.Entity.SameAs(Entity.Disclosure))
            .Where(r => r.EntityAttribute.SameAs(EntityAttribute.Customization))
            .ToArray();

        public static OldBusinessRule[] RemoveProvidersRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ActionTypeID == ActionType.Delete)
            .Where(r => r.Entity.SameAs(Entity.Provider))
            .Where(r => r.EntityAttribute.IsBlank())
            .ToArray();

        public static string ToSqlValue(this int? value) => value.HasValue ? value.Value.ToString() : "NULL";
    }
}
