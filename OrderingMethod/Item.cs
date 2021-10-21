using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;

namespace BusinessRulesMigrator.OrderingMethod
{
    internal class Item
    {
        public OrderingMethodData OrderingMethod { get; set; }

        public OffersSpec Offers { get; set; }
    }
}
