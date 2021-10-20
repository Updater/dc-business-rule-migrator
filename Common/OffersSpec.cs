using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common
{
    internal class OffersSpec
    {
        public OffersSpec() { }

        public int? ProviderId;
        public ByCode ByCode { get; set; }
        public ByProducts ByProducts { get; set; }
    }
}
