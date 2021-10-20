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
using BusinessRulesMigrator.Common.Offers;

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

        public static string GenerateRuleSql(DriverKey key, RuleType ruleType, int MessageTypeId, object data)
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
                        $"'{JsonSerializer.Serialize(data, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull })}'," +
                        $"NULL," + //null for now
                        "GETDATE()," +
                        "NULL," +
                        "1," +
                        "GETDATE()," +
                        "SUSER_SNAME()" +
                    ")";
        }

        public static void PopulateOfferSpecByCodeByProvider(OldBusinessRule rule, OffersSpec spec)
        {
            // By offer code
            if (!string.IsNullOrEmpty(rule.OfferCode) && !string.Equals(rule.OfferCode, "NULL", StringComparison.OrdinalIgnoreCase))
            {
                if (spec.ByCode is null)
                {
                    spec.ByCode = new ByCode
                    {
                        Condition = "IfAny",
                        Codes = new List<string>()
                    };
                }

                if (!spec.ByCode.Codes.Contains(rule.OfferCode))
                {
                    spec.ByCode.Codes.Add(rule.OfferCode);
                }
            }

            // By Offer Category
            if (rule.OfferTypeID.HasValue && Offer.OfferCategories.ContainsKey(rule.OfferTypeID.Value))
            {
                if (spec.ByProducts is null)
                {
                    spec.ByProducts = new ByProducts
                    {
                        Condition = "IfAny",
                        Specs = new List<ProductSpec>()
                    };
                }

                spec.ByProducts.Specs.Add(
                    new ProductSpec
                    {
                        Condition = "HasExactly",
                        Products = new List<Product>{
                                new Product
                                {
                                    Category = Offer.OfferCategories[rule.OfferTypeID.Value]
                                }
                        }
                    });
            }

            // By Offer Type
            if (rule.OfferSubTypeID.HasValue && Offer.OfferTypes.ContainsKey(rule.OfferSubTypeID.Value))
            {
                if (spec.ByProducts is null)
                {
                    spec.ByProducts = new ByProducts
                    {
                        Condition = "IfAny",
                        Specs = new List<ProductSpec>()
                    };
                }
                var type = Offer.OfferTypes[rule.OfferSubTypeID.Value];
                spec.ByProducts.Specs.Add(
                    new ProductSpec
                    {
                        Condition = "HasExactly",
                        Products = new List<Product>{
                                new Product
                                {
                                    Category = Offer.GetOfferCategoryForOfferType(type.Category),
                                    Type = type.Type
                                }
                        }
                    });
            }
        }
    }
}
