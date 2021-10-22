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

        public static OldBusinessRule[] OverrideOrderConfirmationRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.Entity.SameAs(Entity.OrderConfirmation))
            .Where(r => r.EntityAttribute.SameAs(EntityAttribute.ProviderFollowUpMessage) || r.EntityAttribute.SameAs(EntityAttribute.ProviderFollowUpMessageByResultCode))
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

        public static string ToSqlValue(this int? value) => value.HasValue ? value.Value.ToString() : "NULL";

        public static string ToSqlValue(this string value) => value == null ? "NULL" : $"'{value.Replace("'", "''")}'";

    }
}
