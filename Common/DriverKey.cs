using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common
{
    internal class DriverKey
    {
        public int? SourcePlatformID { get; set; }
        public int? PromoID { get; set; }
        public int? UIReferenceDataID { get; set; }
        public int? OriginatorID { get; set; }
        public int? BrokerID { get; set; }
        public int? ProviderID { get; set; }
        public int? DisplayCategoryID { get; set; }
        public int? PostalCodeGroupID { get; set; }
        public int? CampaignTypeId { get; set; }
        public string BundledOfferTypeIDs { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (SourcePlatformID.HasValue ? SourcePlatformID.GetHashCode() : 0);
                hash = hash * 23 + (PromoID.HasValue ? PromoID.GetHashCode() : 0);
                hash = hash * 23 + (UIReferenceDataID.HasValue ? UIReferenceDataID.GetHashCode() : 0);
                hash = hash * 23 + (OriginatorID.HasValue ? OriginatorID.GetHashCode() : 0);
                hash = hash * 23 + (BrokerID.HasValue ? BrokerID.GetHashCode() : 0);
                hash = hash * 23 + (ProviderID.HasValue ? ProviderID.GetHashCode() : 0);
                hash = hash * 23 + (DisplayCategoryID.HasValue ? DisplayCategoryID.GetHashCode() : 0);
                hash = hash * 23 + (PostalCodeGroupID.HasValue ? PostalCodeGroupID.GetHashCode() : 0);
                hash = hash * 23 + (CampaignTypeId.HasValue ? CampaignTypeId.GetHashCode() : 0);
                hash = hash * 23 + (BundledOfferTypeIDs is not null ? BundledOfferTypeIDs.GetHashCode() : 0);

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
    }

    
}
