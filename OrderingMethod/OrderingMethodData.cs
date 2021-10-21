using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridgevine;

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

                hash = hash * 23 + (Type.IsNotBlank() ? Type.GetHashCode() : 0);
                hash = hash * 23 + (ImageUrl.IsNotBlank() ? ImageUrl.GetHashCode() : 0);
                hash = hash * 23 + (ExternalUrl.IsNotBlank() ? ExternalUrl.GetHashCode() : 0);
                hash = hash * 23 + (Instructions.IsNotBlank() ? Instructions.GetHashCode() : 0);

                return hash;
            }
        }

        public bool SameAs(OrderingMethodData other) => other.IsNotNull() && other.GetHashCode() == GetHashCode();

        public override bool Equals(object obj) => SameAs(obj as OrderingMethodData);
    }
}
