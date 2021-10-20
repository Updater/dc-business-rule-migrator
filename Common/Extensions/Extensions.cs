using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessRulesMigrator.Common.Extensions
{
    public static class Extensions
    {
        public static string ToSqlValue(this int? value)
        {
            return value.HasValue ? value.Value.ToString() : "NULL";
        }
        public static string ToSqlValue(this string value)
        {
            return string.IsNullOrEmpty(value) ? $"'{value}'" : "NULL";
        }
    }
}
