using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using Bridgevine;
using Bridgevine.Json;

namespace BusinessRulesMigrator
{
    internal static class Helpers
    {
        public static string GenerateRuleSql(RuleType ruleType, int messageType, DriverKey driver, object data) =>
            "INSERT INTO BusinessRule " +
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
                "Data" +
            ")" +
            "VALUES" +
            "(" +
                $"{(int)ruleType}," +
                $"{messageType}," +
                $"{driver.ProviderId.ToSqlValue()}," +
                $"{driver.PromoId.ToSqlValue()}," +
                $"{driver.CampaignTypeId.ToSqlValue()}," +
                $"{driver.SourcePlatformId.ToSqlValue()}," +
                $"{driver.UIReferenceDataId.ToSqlValue()}," +
                $"{driver.OriginatorId.ToSqlValue()}," +
                $"{driver.DisplayCategoryId.ToSqlValue()}," +
                "NULL," +
                "NULL," +
                (data.IsNull() ? "NULL" : $"'{BvJson.Serialize(data).Replace("'", "''")}'") +
            ")";
    }
}
