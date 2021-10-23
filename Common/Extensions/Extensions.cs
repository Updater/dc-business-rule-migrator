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
        static class Entity
        {
            public const string Offer = "Offer";
            public const string OrderConfirmation = "OrderConfirmation";
            public const string Customization = "Customization";
            public const string Choice = "Choice";
        }

        static class EntityAttribute
        { 
            public const string RevenueRanking = "RevenueRanking";
            public const string OrderingMethod = "OrderingMethod";
            public const string ProviderFollowUpMessage = "ProviderFollowUpMessage";
            public const string ProviderFollowUpMessageByResultCode = "ProviderFollowUpMessageByResultCode";
            public const string Prepopulate = "Prepopulate";
        }

        static class Action
        {
            public const int Replace = 4;
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

        public static bool IsCustomizationPrepopulate(this OldBusinessRule r) =>
            r.Entity.SameAs(Entity.Customization) && r.EntityAttribute.SameAs(EntityAttribute.Prepopulate);

        public static bool IsOverrideCustomization(this OldBusinessRule r) =>
            r.ActionTypeID == Action.Replace &&
            r.Entity.SameAs(Entity.Customization) && 
            r.EntityAttribute.IsNotBlank() &&
            r.EntityAttribute.SameAs(r.CustomizationCode) &&
            r.ChoiceCode.IsBlank();

        public static bool IsOverrideChoice(this OldBusinessRule r) =>
            r.ActionTypeID == Action.Replace &&
            r.Entity.SameAs(Entity.Choice) &&
            r.EntityAttribute.IsNotBlank() &&
            r.EntityAttribute.SameAs(r.ChoiceCode) &&
            r.CustomizationCode.IsNotBlank();

        public static OldBusinessRule[] OverrideValidationGroupRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.IsCustomizationPrepopulate() || r.IsOverrideCustomization() || r.IsOverrideChoice())
            .ToArray();

        public static string ToSqlValue(this int? value) => value.HasValue ? value.Value.ToString() : "NULL";
    }
}
