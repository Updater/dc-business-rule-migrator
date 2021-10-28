using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BusinessRulesMigrator.Common
{
    internal class OrderingMethodSpec
    {
        public string Condition { get; set; }

        public List<string> OrderingMethods { get; set; }
    }
}
