using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BusinessRulesMigrator.Common
{
    internal class ByIDs
    {
        public string Condition { get; set; }
        
        public List<string> IDs { get; set; }
    }
}
