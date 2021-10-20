using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessRulesMigrator.OrderingMethod
{
    internal sealed class OrderingMethodData
    {
        public string Type { get; set; }

        public string ImageUrl { get; set; }

        public string ExternalUrl { get; set; }

        public string Instructions { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + (!string.IsNullOrEmpty(Type) ? Type.GetHashCode() : 0);
                hash = hash * 23 + (!string.IsNullOrEmpty(ImageUrl) ? ImageUrl.GetHashCode() : 0);
                hash = hash * 23 + (!string.IsNullOrEmpty(ExternalUrl) ? ExternalUrl.GetHashCode() : 0);
                hash = hash * 23 + (!string.IsNullOrEmpty(Instructions) ? Instructions.GetHashCode() : 0);

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
    }

    
}
