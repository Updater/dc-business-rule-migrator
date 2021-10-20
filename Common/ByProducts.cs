using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common
{
    internal class ByProducts
    {
        public ByProducts() { }
        public string Condition { get; set; }
        public List<ProductSpec> Specs { get; set; }
    }
}
