using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BusinessRulesMigrator.Common
{
    internal class OffersSpec
    {
        public OffersSpec() 
        {

        }
  
        public ByProvider ByProvider { get; set; }
        public ByCode ByCode { get; set; }
        public ByProducts ByProducts { get; set; }
    }
}
