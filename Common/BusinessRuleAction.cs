using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessRulesMigrator.Common
{
    public enum BusinessRuleAction : int
    {
        Add = 1,
        Group = 2,
        Sort = 3,
        Replace = 4,
        Conditional = 5,
        Delete = 6,
        RegularExpression = 7,
        Mapping = 8,
        Exclusion = 9,
    }
}
