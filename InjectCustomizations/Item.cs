using System;
using System.Collections.Generic;
using System.Text;
using BusinessRulesMigrator.Common;
using System.Linq;
using Bridgevine;

namespace BusinessRulesMigrator.InjectCustomizations
{
    class CustomizationAction
    {
        public List<int> Add { get; set; }

        public List<int> Remove { get; set; }
    }

    class Item
    {
        public CustomizationAction Customizations { get; set; }

        public int? ProviderId { get; set; }

        public OrderingMethodSpec OrderingMethod { get; set; }

        public OffersSpec OffersInCart { get; set; }

        public void AddOrderingMethodsConstraint(IEnumerable<string> orderingMethods)
        {
            if (!orderingMethods.Safe().Any()) return;

            OrderingMethod ??= new OrderingMethodSpec 
            {
                Condition = "IfAny",
                OrderingMethods = new List<string>(),
            };

            var add = orderingMethods.Where(om1 => !OrderingMethod.OrderingMethods.Any(om2 => om1.SameAs(om2))).ToList();

            OrderingMethod.OrderingMethods.AddRange(add);
        }

        public void AddOfferConstraint(string offerCode)
        {
            if (offerCode.IsBlank()) return;

            OffersInCart ??= new OffersSpec();

            OffersInCart.AddOfferCode(offerCode);
        }

        public void HandleCustomizations(bool add, IEnumerable<int> ids)
        {
            if (!ids.Safe().Any()) return;

            Customizations ??= new CustomizationAction();

            if (add)
            {
                Customizations.Add = Customizations.Add.Safe().Concat(ids).Distinct().ToList();
            }
            else
            {
                Customizations.Remove = Customizations.Remove.Safe().Concat(ids).Distinct().ToList();
            }
        }
    }
}
