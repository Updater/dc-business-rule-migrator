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
using BusinessRulesMigrator.OrderingMethod;
using BusinessRulesMigrator.OverrideOrderConfirmation;
using BusinessRulesMigrator.OverrideOffer;
using BusinessRulesMigrator.OverrideValidationGroup;
using BusinessRulesMigrator.ConditionalProviderAvailability;
using BusinessRulesMigrator.InjectCustomizations;
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
                    var rules = csv.GetRecords<OldBusinessRule>().ToArray();

                    var revenueRankingRules = new RevenueRankingRuleConverter().Convert(rules);
                    var orderingMethodRules = new OrderingMethodRuleConverter().Convert(rules);
                    var overrideOrderConfirmationRules = new OverrideOrderConfirmationConverter().Convert(rules);
                    var overrideOfferRules = new OverrideOfferConverter().Convert(rules);
                    var overrideValidationGroupRules = new OverrideValidationGroupConverter().Convert(rules);
                    var conditionalProviderAvailabilityRules = new ConditionalProviderAvailabilityConverter().Convert(rules);
                    var injectCustomizationsRules = new InjectCustomizationsConverter().Convert(rules);
                    var injectDisclosureCustomizationsRules = new InjectDisclosureCustomizationsConverter().Convert(rules);

                    var newRules =
                        revenueRankingRules
                        .Concat(orderingMethodRules)
                        .Concat(overrideOrderConfirmationRules)
                        .Concat(overrideOfferRules)
                        .Concat(overrideValidationGroupRules)
                        .Concat(conditionalProviderAvailabilityRules)
                        .Concat(injectCustomizationsRules)
                        .Concat(injectDisclosureCustomizationsRules)
                        .ToArray();

                    if (newRules.Any())
                    {
                        File.WriteAllLines(@"C:\BusinessRules\ConvertedRules.txt", newRules);
                        Console.WriteLine("Success!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
