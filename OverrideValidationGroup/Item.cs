using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using Bridgevine;

namespace BusinessRulesMigrator.OverrideValidationGroup
{
    class ResultCodesSet
    {
        public string Condition { get; set; }

        public List<string> Codes { get; set; }

        public void AddResultCode(string code)
        {
            if (code.IsBlank() || code.SameAs("NULL")) return;

            Codes ??= new List<string>();

            if (!Codes.Any(c => c.SameAs(code)))
            {
                Codes.Add(code);
            }
        }
    }

    class ResultCodesSpec
    {
        public string Condition { get; set; }

        public List<ResultCodesSet> Specs { get; set; }
    }

    class ConfirmationSpec
    {
        public ResultCodesSpec ResultCodes { get; set; }

        public string LogicalOperator { get; set; }

        public OfferAvailabilitySpec Offers { get; set; }

        public void AddOfferCode(string code)
        {
            if (code.IsBlank() || code.SameAs("NULL")) return;

            if (Offers.IsNull())
            {
                Offers = new OfferAvailabilitySpec 
                {
                    Condition = "IfAny",
                    Specs = new List<OffersSpec> 
                    {
                        new OffersSpec(),
                    },
                };
            }

            Offers.Specs.First().AddOfferCode(code);
        }

        public void AddResultCode(string code)
        {
            if (code.IsBlank() || code.SameAs("NULL")) return;

            if (ResultCodes.IsNull())
            {
                ResultCodes = new ResultCodesSpec
                {
                    Condition = "IfAny",
                    Specs = new List<ResultCodesSet>
                    {
                        new ResultCodesSet
                        {
                           Condition = "IfAny",
                        },
                    },
                };
            }

            ResultCodes.Specs.First().AddResultCode(code);
        }
    }

    class Item
    {
        public string Message { get; set; }

        public int Priority { get; set; }

        public ConfirmationSpec Criteria { get; set; }

        public void UpdateCriteria()
        {
            if (Criteria.IsNull()) return;

            var hasOfferSpecs = (Criteria.Offers?.Specs).Safe().Any(s => s.HasConstraints());
            var hasResultCodesSpecs = (Criteria.ResultCodes?.Specs).Safe().Any(s => s.Codes.Safe().Any());

            if (!hasOfferSpecs && !hasResultCodesSpecs)
                Criteria = null;
        }
    }
}
