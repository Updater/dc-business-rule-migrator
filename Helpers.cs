using System;
using System.Text.Json;
using System.Collections.Generic;
using BusinessRulesMigrator.Common;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper;
using BusinessRulesMigrator.Common.CsvHelper;
using BusinessRulesMigrator.Common.Extensions;
using BusinessRulesMigrator.RevenueRanking;

namespace BusinessRulesMigrator
{
    internal static class Helpers
    {
        public static DriverKey GetKey(OldBusinessRule rule)
        {
            return new DriverKey
            {
                SourcePlatformID = rule.SourcePlatformID,
                PromoID = rule.PromoID,
                UIReferenceDataID = rule.UIReferenceDataID,
                OriginatorID = rule.OriginatorID,
                BrokerID = rule.BrokerID,
                ProviderID = rule.ProviderID,
                DisplayCategoryID = rule.DisplayCategoryID,
                PostalCodeGroupID = rule.PostalCodeGroupID,
                CampaignTypeId = rule.CampaignTypeId,
                BundledOfferTypeIDs = rule.BundledOfferTypeIDs
            };
        }

        public static string GenerateRuleSql(DriverKey key, RuleType ruleType, int MessageTypeId, string data)
        {
            if (key is null) throw new ArgumentNullException("rule");

            return "INSERT INTO BusinessRule " +
                    "(" +
                        "BusinessRuleTypeId," +
                        "BrokerMessageId," +
                        "ProviderId," +
                        "PromoId," +
                        "CampaignTypeId," +
                        "SourcePlatformId," +
                        "UIReferenceDataId," +
                        "OriginatorId," +
                        "DisplayCategoryId," +
                        "StateGroupId," +
                        "ZipCodeGroupId," +
                        "Data," +
                        "Condition," +
                        "ValidFrom," +
                        "ValidTo," +
                        "Active," +
                        "CreateDate," +
                        "CreatedBy" +
                    ")" +
                    "VALUES" +
                    "(" +
                        $"{(int)ruleType}," +
                        $"{MessageTypeId}," +
                        $"{key.ProviderID.ToSqlValue()}," +
                        $"{key.PromoID.ToSqlValue()}," +
                        $"{key.CampaignTypeId.ToSqlValue()}," +
                        $"{key.SourcePlatformID.ToSqlValue()}," +
                        $"{key.UIReferenceDataID.ToSqlValue()}," +
                        $"{key.OriginatorID.ToSqlValue()}," +
                        $"{key.DisplayCategoryID.ToSqlValue()}," +
                        "NULL," + //null for now
                        "NULL," + //null for now
                        $"'{data}'," +
                        $"NULL," + //null for now
                        "GETDATE()," +
                        "NULL," +
                        "1," +
                        "GETDATE()," +
                        "SUSER_SNAME()" +
                    ")";
        }
    }
}
