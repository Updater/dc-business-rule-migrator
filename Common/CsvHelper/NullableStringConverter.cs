using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace BusinessRulesMigrator.Common.CsvHelper
{
    internal class NullableStringConverter : StringConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.Equals(text, "NULL", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            else
            {
                return base.ConvertFromString(text, row, memberMapData);
            }
            
        }
    }
}
