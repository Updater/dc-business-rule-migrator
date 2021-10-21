using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BusinessRulesMigrator.Common
{
    internal class ByProvider
    {
        public ByProvider() { }
        public string Condition { get; set; }
        public List<int> IDs { get; set; }
    }
}
