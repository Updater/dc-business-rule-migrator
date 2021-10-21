using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common
{
    internal class ProductSpec
    {
        public string Condition { get; set; }

        public List<Product> Products { get; set; }
    }
}
