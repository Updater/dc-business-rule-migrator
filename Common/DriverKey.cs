using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common
{
    internal class DriverKey
    {
        public int? ProviderId { get; set; }

        public int? PromoId { get; set; }

        public int? CampaignTypeId { get; set; }

        public int? SourcePlatformId { get; set; }

        public int? UIReferenceDataId { get; set; }

        public int? OriginatorId { get; set; }

        public int? DisplayCategoryId { get; set; }

        public int? PostalCodeGroupId { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + (ProviderId.HasValue ? ProviderId.GetHashCode() : 0);
                hash = hash * 23 + (PromoId.HasValue ? PromoId.GetHashCode() : 0);
                hash = hash * 23 + (CampaignTypeId.HasValue ? CampaignTypeId.GetHashCode() : 0);
                hash = hash * 23 + (SourcePlatformId.HasValue ? SourcePlatformId.GetHashCode() : 0);
                hash = hash * 23 + (UIReferenceDataId.HasValue ? UIReferenceDataId.GetHashCode() : 0);
                hash = hash * 23 + (OriginatorId.HasValue ? OriginatorId.GetHashCode() : 0);
                hash = hash * 23 + (DisplayCategoryId.HasValue ? DisplayCategoryId.GetHashCode() : 0);
                hash = hash * 23 + (PostalCodeGroupId.HasValue ? PostalCodeGroupId.GetHashCode() : 0);

                return hash;
            }
        }

        public override bool Equals(object obj) => obj.GetHashCode() == GetHashCode();
    }
}
