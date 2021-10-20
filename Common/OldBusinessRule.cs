﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common
{
    internal class OldBusinessRule
    {
        public int BusinessRuleID { get; set; }
        public int? SourcePlatformID { get; set; }
        public int? PromoID { get; set; }
        public int? UIReferenceDataID { get; set; }
        public int? OriginatorID { get; set; }
        public int? BrokerID { get; set; }
        public int? ProviderID { get; set; }
        public int? OfferTypeID { get; set; }
        public int? OfferSubTypeID { get; set; }
        public string OfferCode { get; set; }
        public string CustomizationCode { get; set; }
        public string ChoiceCode { get; set; }
        public int? DisplayCategoryID { get; set; }
        public int? PostalCodeGroupID { get; set; }
        public string Condition { get; set; }
        public string value { get; set; }
        public int? CampaignTypeId { get; set; }
        public string BundledOfferTypeIDs { get; set; }
        public string StateCode { get; set; }
    }
}