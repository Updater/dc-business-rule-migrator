using System;
using System.Text.Json;
using System.Collections.Generic;
using BusinessRulesMigrator.Common;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper;
using BusinessRulesMigrator.Common.CsvHelper;
using BusinessRulesMigrator.Common.Extensions;
using BusinessRulesMigrator.RevenueRanking;
using System.Threading.Tasks;

namespace BusinessRulesMigrator
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            try
            {
                using (var reader = new StreamReader(@"C:\BusinessRules\OldRules.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.TypeConverterCache.AddConverter<int?>(new NullableInt32Converter());
                    csv.Context.TypeConverterCache.AddConverter<string>(new NullableStringConverter());
                    var rules = csv.GetRecords<OldBusinessRule>();
                    Console.WriteLine("Converting...");

                    var converted = new RevenueRankingRuleConverter().ConvertRules(rules);

                    if (converted is not null && converted.Any())
                    {
                        File.WriteAllLines(@"C:\BusinessRules\ConvertedRules.txt", converted);
                        Console.WriteLine("Success!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.ToString()}");
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        
    }
}
