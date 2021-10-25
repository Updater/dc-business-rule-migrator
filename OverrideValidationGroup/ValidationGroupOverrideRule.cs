using System;
using System.Collections.Generic;
using System.Text;

using BusinessRulesMigrator.Common;

namespace BusinessRulesMigrator.OverrideValidationGroup
{
    class ValidationGroupOverrideRule
    {
        public int ValidationGroupOverrideID { get; set; }

        public int ProviderID { get; set; }

        public string ValidationGroupCode { get; set; }

        public int? PromoID { get; set; }

        public int? SourcePlatformID { get; set; }

        public int? CampaignTypeID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DriverKey GetDriverKey() => new DriverKey
        {
            ProviderId = ProviderID,
            PromoId = PromoID,
            CampaignTypeId = CampaignTypeID,
            SourcePlatformId = SourcePlatformID,
            UIReferenceDataId = default,
            OriginatorId = default,
            DisplayCategoryId = default,
            PostalCodeGroupId = default,
        };
    }
}
