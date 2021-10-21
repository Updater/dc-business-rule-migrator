using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;

namespace BusinessRulesMigrator.OverrideOrderConfirmation
{
    class ResultCodesSet
    {
        public string Condition { get; set; }

        public List<string> Codes { get; set; }
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
    }

    class Item
    {
        public string Message { get; set; }

        public int Priority { get; set; }

        public ConfirmationSpec Criteria { get; set; }
    }
}
