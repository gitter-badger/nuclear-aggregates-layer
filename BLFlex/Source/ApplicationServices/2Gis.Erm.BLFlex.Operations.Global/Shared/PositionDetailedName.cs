using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public static class PositionDetailedName
    {
        public static readonly Func<Key, IEnumerable<Advertisement>, string> FormatOldSalesModels = Format<OldSalesModels>;
        public static readonly Func<Key, IEnumerable<Advertisement>, string> FormatMultiFullHouse = Format<MultiFullHouse>;

        private const string BindingObjectSeparator = ", ";
        private const string CompositeItemsSeparator = "; ";
        private const string NameDetailsSeparator = ", ";

        private interface ISalesModelPositionNameFormatter
        {
            string Format(IEnumerable<Advertisement> values);
        }

        private static string Format<T>(Key key, IEnumerable<Advertisement> values)
            where T : ISalesModelPositionNameFormatter, new()
        {
            var singleBindingObject = values.GroupBy(advertisement => advertisement.PositionName)
                                            .Distinct(new BindingGroupComparer()).Count() == 1;
            var formatter = new T();
            var prefix = key.OrderPositionName;
            var details = key.IsComposite && !singleBindingObject 
                ? FormatComposite(formatter, values) 
                : formatter.Format(values);
            return Compose(NameDetailsSeparator, prefix, details);
        }

        private static string FormatComposite(ISalesModelPositionNameFormatter formatter, IEnumerable<Advertisement> values)
        {
            var simples = values.GroupBy(value => value.PositionName)
                                .Select(group => FormatCompositePart(formatter, group.Key, group))
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .ToArray();

            return simples.Any()
                       ? BLResources.PositionNameFormatCompositeDetalization + string.Join(CompositeItemsSeparator, simples)
                       : string.Empty;
        }

        private static string FormatCompositePart(ISalesModelPositionNameFormatter formatter, string name, IEnumerable<Advertisement> values)
        {
            var detalization = formatter.Format(values);
            return string.IsNullOrWhiteSpace(detalization) ? string.Empty : Compose(NameDetailsSeparator, name, detalization);
        }

        private static string Compose(string separator, string requred, string optional)
        {
            return string.IsNullOrWhiteSpace(optional)
                       ? requred
                       : requred + separator + optional;
        }

        private static string SelectThemeQuoted(this IEnumerable<Advertisement> values)
        {
            return string.Join(BindingObjectSeparator, values.Select(ThemeQuoted).Distinct());
        }

        private static string ThemeQuoted(Advertisement value)
        {
            return string.Format(BLResources.QuotedValue, value.ThemeName);
        }

        private static string SelectAddressQuoted(this IEnumerable<Advertisement> values)
        {
            return string.Join(BindingObjectSeparator, values.Select(AddressQuoted).Distinct());
        }

        private static string AddressQuoted(Advertisement value)
        {
            return string.IsNullOrEmpty(value.ReferencePoint)
                ? string.Format(BLResources.QuotedValue, value.Address)
                : string.Format(BLResources.QuotedValue, string.Format(BLResources.AddressWithReferencePoint, value.Address, value.ReferencePoint));
        }

        private static string SelectCategoryQuoted(this IEnumerable<Advertisement> values)
        {
            return string.Join(BindingObjectSeparator, values.Select(CategoryQuoted).Distinct());
        }
        
        private static string CategoryQuoted(Advertisement value)
        {
            return string.Format(BLResources.QuotedValue, value.CategoryName);
        }

        private class OldSalesModels : ISalesModelPositionNameFormatter
        {
            string ISalesModelPositionNameFormatter.Format(IEnumerable<Advertisement> values)
            {
                // ReSharper disable PossibleMultipleEnumeration
                var binding = values.Select(x => x.BindingType).Distinct().Single();

                switch (binding)
                {
                    case PositionBindingObjectType.Firm:
                        return string.Empty;
                    case PositionBindingObjectType.AddressSingle:
                        return string.Format(BLResources.PositionNameFormatAddressSingle,
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.AddressMultiple:
                        return string.Format(BLResources.PositionNameFormatAddressMultiple,
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.CategorySingle:
                        return string.Format(BLResources.PositionNameFormatCategorySingle,
                                             values.SelectCategoryQuoted());
                    case PositionBindingObjectType.AddressCategorySingle:
                        return string.Format(BLResources.PositionNameFormatAddressCategorySingle,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.AddressCategoryMultiple:
                        return string.Format(BLResources.PositionNameFormatAddressCategoryMultiple,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.CategoryMultiple:
                        return string.Format(BLResources.PositionNameFormatCategoryMultiple,
                                             values.SelectCategoryQuoted());
                    case PositionBindingObjectType.AddressFirstLevelCategorySingle:
                        return string.Format(BLResources.PositionNameFormatAddressFirstLevelCategorySingle,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.AddressFirstLevelCategoryMultiple:
                        return string.Format(BLResources.PositionNameFormatAddressFirstLevelCategoryMultiple,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.CategoryMultipleAsterix:
                        return string.Format(BLResources.PositionNameFormatCategoryMultipleAsterix,
                                             values.SelectCategoryQuoted());
                    case PositionBindingObjectType.ThemeMultiple:
                        return string.Format(BLResources.PositionNameFormatThemeMultiple,
                                             values.SelectThemeQuoted());
                    default:
                        throw new BusinessLogicException(string.Format(BLResources.UnsupportedPositionBindingObjectType, binding));
                }
                // ReSharper restore PossibleMultipleEnumeration
            }
        }

        private class MultiFullHouse : ISalesModelPositionNameFormatter
        {
            string ISalesModelPositionNameFormatter.Format(IEnumerable<Advertisement> values)
            {
                // ReSharper disable PossibleMultipleEnumeration
                var binding = values.Select(x => x.BindingType).Distinct().Single();

                switch (binding)
                {
                    case PositionBindingObjectType.Firm:
                        return string.Empty;
                    case PositionBindingObjectType.AddressSingle:
                        return string.Format(BLResources.PositionNameFormatAddressSingle,
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.AddressMultiple:
                        return string.Format(BLResources.PositionNameFormatAddressMultiple,
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.CategorySingle:
                        return string.Format(BLResources.PositionNameFormatContextCategorySingle,
                                             values.SelectCategoryQuoted());
                    case PositionBindingObjectType.AddressCategorySingle:
                        return string.Format(BLResources.PositionNameFormatAddressContextCategorySingle,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.AddressCategoryMultiple:
                        return string.Format(BLResources.PositionNameFormatAddressContextCategoryMultiple,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.CategoryMultiple:
                        return string.Format(BLResources.PositionNameFormatContextCategoryMultiple,
                                             values.SelectCategoryQuoted());
                    case PositionBindingObjectType.AddressFirstLevelCategorySingle:
                        return string.Format(BLResources.PositionNameFormatAddressFirstLevelContextCategorySingle,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.AddressFirstLevelCategoryMultiple:
                        return string.Format(BLResources.PositionNameFormatAddressFirstLevelContextCategoryMultiple,
                                             values.SelectCategoryQuoted(),
                                             values.SelectAddressQuoted());
                    case PositionBindingObjectType.CategoryMultipleAsterix:
                        return string.Format(BLResources.PositionNameFormatContextCategoryMultipleAsterix,
                                             values.SelectCategoryQuoted());
                    case PositionBindingObjectType.ThemeMultiple:
                        return string.Format(BLResources.PositionNameFormatThemeMultiple,
                                             values.SelectThemeQuoted());
                    default:
                        throw new BusinessLogicException(string.Format(BLResources.UnsupportedPositionBindingObjectType, binding));
                }
                // ReSharper restore PossibleMultipleEnumeration
            }
        }

        public sealed class Key
        {
            public string OrderPositionName { get; set; }
            public bool IsComposite { get; set; }
        }

        public sealed class Advertisement
        {
            public PositionBindingObjectType BindingType { get; set; }
            public string PositionName { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
            public string ThemeName { get; set; }
            public string CategoryName { get; set; }
        }

        private class BindingGroupComparer : IEqualityComparer<IGrouping<string, Advertisement>>
        {
            private readonly BindingComparer _bindingComparer = new BindingComparer();

            public bool Equals(IGrouping<string, Advertisement> x, IGrouping<string, Advertisement> y)
            {
                return x.Count() == y.Count()
                       && x.All(advertisement => y.Contains(advertisement, _bindingComparer));
            }

            public int GetHashCode(IGrouping<string, Advertisement> group)
            {
                return group.Aggregate(0, (acc, advertisement) => acc ^ _bindingComparer.GetHashCode());
            }
        }

        private class BindingComparer : IEqualityComparer<Advertisement>
        {
            public bool Equals(Advertisement x, Advertisement y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(null, x))
                {
                    return false;
                }

                if (ReferenceEquals(null, y))
                {
                    return false;
                }

                return x.BindingType == y.BindingType
                    && string.Equals(x.Address, y.Address)
                    && string.Equals(x.ReferencePoint, y.ReferencePoint)
                    && string.Equals(x.ThemeName, y.ThemeName)
                    && string.Equals(x.CategoryName, y.CategoryName);
            }

            public int GetHashCode(Advertisement obj)
            {
                unchecked
                {
                    var hashCode = (int)obj.BindingType;
                    hashCode = (hashCode * 397) ^ (obj.Address != null ? obj.Address.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.ReferencePoint != null ? obj.ReferencePoint.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.ThemeName != null ? obj.ThemeName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.CategoryName != null ? obj.CategoryName.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}
