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
        }

        static class EntityAttribute
        { 
            public const string RevenueRanking = "RevenueRanking";
            public const string ProviderFollowUpMessage = "ProviderFollowUpMessage";
            public const string ProviderFollowUpMessageByResultCode = "ProviderFollowUpMessageByResultCode";
        }

        public static OldBusinessRule[] RevenueRankingRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.Entity.SameAs(Entity.Offer) && r.EntityAttribute.SameAs(EntityAttribute.RevenueRanking))
            .ToArray();

        public static OldBusinessRule[] OverrideOrderConfirmationRules(this IEnumerable<OldBusinessRule> rules) =>
            rules
            .Where(r => r.ProviderID.HasValue)
            .Where(r => r.Entity.SameAs(Entity.OrderConfirmation))
            .Where(r => r.EntityAttribute.SameAs(EntityAttribute.ProviderFollowUpMessage) || r.EntityAttribute.SameAs(EntityAttribute.ProviderFollowUpMessageByResultCode))
            .ToArray();

        public static string ToSqlValue(this int? value) => value.HasValue ? value.Value.ToString() : "NULL";

        public static string ToSqlValue(this string value) => value == null ? "NULL" : $"'{value}'";

    }
}
