using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessRulesMigrator.Common;
using BusinessRulesMigrator.Common.Extensions;
using BusinessRulesMigrator.Common.Offers;
using Bridgevine;

namespace BusinessRulesMigrator.OverrideOffer
{
    internal sealed class PromotionItem
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Action { get; set; }

        public string TargetId { get; set; }
    }

    internal sealed class PromotionData
    {
        public bool RemoveAll { get; set; }

        public List<PromotionItem> Items { get; set; }

        public bool HasAny() => Items.Safe().Any();
    }

    internal sealed class PriceData
    {
        public int? Id { get; set; }

        public string Code { get; set; }

        public string Action { get; set; }

        public string Category { get; set; }
    }

    internal sealed class OrderingMethodData
    {
        public string Type { get; set; }

        public string ImageUrl { get; set; }

        public string ExternalUrl { get; set; }

        public string Instructions { get; set; }
    }

    internal class OfferDataBase
    {
        public string RevenueRanking { get; set; }

        public List<string> Tags { get; set; }

        public bool HasTags() => Tags.Safe().Any();

        public bool HasTag(string tag) => Tags.Any(t => t.SameAs(tag));

        public List<string> Bonuses { get; set; }

        public PromotionData Promotions { get; set; }

        public List<PriceData> Prices { get; set; }

        public OrderingMethodData OrderingMethod { get; set; }

        public virtual void OverrideOfferProperty(string propertyName, string value, int actionTypeId)
        {
            switch (propertyName)
            {
                case Extensions.Entity.Bonus:
                    AddBonus(value);
                    break;
                case Extensions.Entity.Promotion:
                    OverridePromotion(value, actionTypeId);
                    break;
                case Extensions.EntityAttribute.ShortTermPrice:
                case Extensions.EntityAttribute.DepositPrice:
                case Extensions.EntityAttribute.HardwarePrice:
                case Extensions.EntityAttribute.InstallPrice:
                    OverridePrice(propertyName, value, actionTypeId);
                    break;
            }
        }

        private void AddBonus(string value)
        {
            if (value.IsBlank()) return;

            if (Bonuses.IsNull())
            {
                Bonuses = new List<string>();
            }

            Bonuses.Add(value);
        }

        private void OverridePromotion(string value, int actionTypeId)
        {
            if (value.IsBlank()) return;

            var add = false;
            var pi = new PromotionItem();

            switch (actionTypeId)
            {
                case (int)BusinessRuleAction.Add:
                    pi.Action = "Add";
                    if (int.TryParse(value, out var pid))
                    {
                        pi.Id = value;
                        add = true;
                    }
                    break;

                case (int)BusinessRuleAction.Replace:
                    pi.Action = "ReplaceExistingOne";
                    if (int.TryParse(value, out pid))
                    {
                        pi.Id = value;
                        add = true;
                    }
                    break;

                case (int)BusinessRuleAction.Delete:
                    Promotions.RemoveAll = true;
                    add = true;
                    break;
            }


            if (add)
            {
                if (Promotions.IsNull())
                {
                    Promotions = new PromotionData { Items = new List<PromotionItem>() };
                }

                Promotions.Items.Add(pi);
            }
            
        }

        private void OverridePrice(string category, string value, int actionTypeId)
        {
            if (Prices.IsNull())
            {
                Prices = new List<PriceData>();
            }

            var add = false;
            var pd = new PriceData { Category = category };

            switch (actionTypeId)
            {
                case (int)BusinessRuleAction.Add:
                    pd.Action = "AddIfNoneExists";
                    if (int.TryParse(value, out int pid))
                    {
                        pd.Id = pid;
                        add = true;
                    }
                    
                    break;
                case (int)BusinessRuleAction.Delete:
                    pd.Action = "Remove";
                    add = true;
                    break;
            }

            if (add)
            {
                Prices.Add(pd);
            }
        }
    }

    internal sealed class OfferData : OfferDataBase
    {
        public int? ProviderId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string MediumName { get; set; }

        public string LongName { get; set; }

        public bool? Display { get; set; }

        public string Logo { get; set; }

        public string LargeLogo { get; set; }

        public string DefaultHighlight { get; set; }

        public string ShortHighlight1 { get; set; }

        public string ShortHighlight2 { get; set; }

        public string ShortHighlight3 { get; set; }

        public string ShortHighlight4 { get; set; }

        public string Disclaimer { get; set; }

        public string TermsAndConditions { get; set; }

        public string BenefitStatementLong { get; set; }

        public string TechSupport { get; set; }

        public string QualifiedHardware { get; set; }

        public override void OverrideOfferProperty(string propertyName, string value, int actionTypeId)
        {
            switch (propertyName)
            {
                case Extensions.Entity.Bonus:
                case Extensions.Entity.Promotion:
                case Extensions.EntityAttribute.DepositPrice:
                case Extensions.EntityAttribute.HardwarePrice:
                case Extensions.EntityAttribute.InstallPrice:
                case Extensions.EntityAttribute.ShortTermPrice:
                    base.OverrideOfferProperty(propertyName, value, actionTypeId);
                    break;
                default:
                    var prop = typeof(OfferData).GetProperty(propertyName);
                    if (prop.IsNotNull())
                    {
                        prop.SetValue(this, value);
                    }
                    break;
            }
        }
    }

    internal class OverrideOfferByProducts
    {
        public ByProducts Filter { get; set; }

        public OfferDataBase OfferData { get; set; }

        public void AddProduct(int? offerTypeId, int? offerSubTypeId, string propertyName, string value, int actionTypeId)
        {
            if (OfferData.IsNull())
            {
                OfferData = new OfferDataBase();
            }

            OfferData.OverrideOfferProperty(propertyName, value, actionTypeId);

            if (offerTypeId.HasValue)
            {
                AddProductByCategory(offerTypeId.Value);
            }
            else
            {
                AddProductByType(offerSubTypeId.Value);
            }
        }

        private void AddProductByCategory(int category)
        {
            if (Filter.IsNull())
            {
                Filter = new ByProducts
                {
                    Condition = "IfAny",
                    Specs = new List<ProductSpec>()
                };
            }

            Filter.Specs.Add(new ProductSpec
            {
                Condition = "HasExactly",
                Products = new List<Product>
                {
                    new Product
                    {
                        Category = Offer.OfferCategories[category]
                    }
                }
            });
        }

        private void AddProductByType(int type)
        {
            if (Filter.IsNull())
            {
                Filter = new ByProducts
                {
                    Condition = "IfAny",
                    Specs = new List<ProductSpec>()
                };
            }

            var r = Offer.OfferTypes[type];

            Filter.Specs.Add(new ProductSpec
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

    internal class OverrideOffer
    {
        public List<OfferData> ByCode { get; set; }

        public List<OverrideOfferByProducts> ByProducts { get; set; }

        public void OverrideByCode(int providerId, string code, string propertyName, string value, int actionTypeId)
        {
            if (code.IsBlank() || code.SameAs("NULL")) return;

            OfferData offerData;
            if (ByCode.IsNull())
            {
                offerData = new OfferData { ProviderId = providerId, Code = code };
                ByCode = new List<OfferData> { offerData };
            }
            else
            {
                offerData = ByCode.FirstOrDefault(od => od.Code.SameAs(code));
                if (offerData.IsNull())
                {
                    offerData = new OfferData { ProviderId = providerId, Code = code };
                    ByCode.Add(offerData);
                }
            }

            offerData.OverrideOfferProperty(propertyName, value, actionTypeId);
        }

        public void OverrideByProduct(int? offerTypeId, int? offerSubTypeId, string propertyName, string value, int actionTypeId)
        {
            if (!offerTypeId.HasValue && !offerSubTypeId.HasValue) return;

            if (ByProducts.IsNull())
            {
                ByProducts = new List<OverrideOfferByProducts>();
            }

            var obp = new OverrideOfferByProducts();
            obp.AddProduct(offerTypeId, offerSubTypeId, propertyName, value, actionTypeId);
        }
    }
    
}
