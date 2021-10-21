﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Bridgevine;
using BusinessRulesMigrator.Common.Offers;

namespace BusinessRulesMigrator.Common
{
    internal class OffersSpec
    {   
        public ByIDs ByProvider { get; set; }

        public ByCode ByCode { get; set; }
     
        public ByProducts ByProducts { get; set; }

        public void AddOfferCode(string code)
        {
            if (code.IsBlank() || code.SameAs("NULL")) return;

            if (ByCode.IsNull())
            {
                ByCode = new ByCode
                {
                    Condition = "IfAny",
                    Codes = new List<string>()
                };
            }

            if (!ByCode.Codes.Any(c => c.SameAs(code)))
            {
                ByCode.Codes.Add(code);
            }
        }

        public void AddProductByCategory(int? category)
        {
            if (!category.HasValue || !Offer.OfferCategories.ContainsKey(category.Value)) return;

            if (ByProducts.IsNull())
            {
                ByProducts = new ByProducts
                {
                    Condition = "IfAny",
                    Specs = new List<ProductSpec>()
                };
            }

            ByProducts.Specs.Add(new ProductSpec
            {
                Condition = "HasExactly",
                Products = new List<Product>
                {
                    new Product
                    {
                        Category = Offer.OfferCategories[category.Value]
                    }
                }
            });
        }

        public void AddProductByType(int? type)
        {
            if (!type.HasValue || !Offer.OfferTypes.ContainsKey(type.Value)) return;

            if (ByProducts.IsNull())
            {
                ByProducts = new ByProducts
                {
                    Condition = "IfAny",
                    Specs = new List<ProductSpec>()
                };
            }

            var r = Offer.OfferTypes[type.Value];
            
            ByProducts.Specs.Add(new ProductSpec
            {
                Condition = "HasExactly",
                Products = new List<Product>
                {
                    new Product
                    {
                        Category = Offer.GetOfferCategoryForOfferType(r.Category),
                        Type = r.Type
                    }
                }
            });
        }
    }
}
