using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BusinessRulesMigrator.Common
{
    internal class ByCode
    {
        public string Condition { get; set; }

        public List<string> Codes { get; set; }
    }
}
