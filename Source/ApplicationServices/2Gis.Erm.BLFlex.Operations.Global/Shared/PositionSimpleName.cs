using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public sealed class PositionSimpleName
    {
        private static readonly PositionBindingObjectType[] CategoryBindedTypes =
            {
                PositionBindingObjectType.CategoryMultipleAsterix,
                PositionBindingObjectType.AddressCategorySingle,
                PositionBindingObjectType.AddressCategoryMultiple,
                PositionBindingObjectType.CategorySingle,
                PositionBindingObjectType.CategoryMultiple,
                PositionBindingObjectType.AddressFirstLevelCategorySingle,
                PositionBindingObjectType.AddressFirstLevelCategoryMultiple,
            };

        public static string FormatName(bool isPositionComposite, PositionBindingObjectType bindingType, string positionName, IEnumerable<AdvertisementDto> advertisements)
        {
            if (isPositionComposite)
            {
                if (CategoryBindedCompositePosition(bindingType))
                {
                    var categories = advertisements.SelectMany(z => z.Categories)
                                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                                   .Distinct()
                                                   .ToArray();
                    return categories.Any()
                               ? string.Format(BLResources.OrderPositionNameWithContextCategory, positionName, string.Join(", ", categories))
                               : positionName;
                }

                return positionName;
            }

            var bindings = advertisements
                .Select(z =>
                            {
                                var address = FormatAddressWithReferencePoint(z.Address, z.ReferencePoint);
                                var categories = z.Categories.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                                return !categories.Any()
                                           ? address
                                           : string.IsNullOrEmpty(address)
                                                 ? string.Join(", ", categories)
                                                 : string.Format("{0}: {1}", address, string.Join(", ", categories));
                            })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            return bindings.Any()
                       ? positionName + ": " + string.Join(", ", bindings)
                       : positionName;
        }

        private static bool CategoryBindedCompositePosition(PositionBindingObjectType bindingType)
        {
            return CategoryBindedTypes.Contains(bindingType);
        }

        private static string FormatAddressWithReferencePoint(string address, string referencePoint)
        {
            return string.IsNullOrWhiteSpace(referencePoint)
                       ? address
                       : string.Format(BLResources.AddressWithReferencePoint, address, referencePoint);
        }

        public sealed class AdvertisementDto
        {
            public IEnumerable<string> Categories { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
        }
    }
}