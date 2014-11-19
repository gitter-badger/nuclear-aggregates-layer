﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public static class PositionDetailedName
    {
        private const string BindingObjectSeparator = ", ";
        private const string CompositeItemsSeparator = "; ";
        private const string NameDetailsSeparator = ", ";

        public static string Format(Key key, IEnumerable<Advertisement> values)
        {
            var prefix = key.OrderPositionName;
            var details = key.IsComposite ? FormatComposite(values) : FormatSimple(values);
            return Compose(NameDetailsSeparator, prefix, details);
        }

        private static string FormatComposite(IEnumerable<Advertisement> values)
        {
            var simples = values.GroupBy(value => value.PositionName)
                                .Select(group => FormatCompositePart(group.Key, group))
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .ToArray();

            return simples.Any()
                       ? BLResources.PositionNameFormatCompositeDetalization + string.Join(CompositeItemsSeparator, simples)
                       : string.Empty;
        }

        private static string FormatCompositePart(string name, IEnumerable<Advertisement> values)
        {
            var detalization = FormatSimple(values);
            return string.IsNullOrWhiteSpace(detalization) ? string.Empty : Compose(NameDetailsSeparator, name, detalization);
        }

        private static string Compose(string separator, string requred, string optional)
        {
            return string.IsNullOrWhiteSpace(optional)
                       ? requred
                       : requred + separator + optional;
        }

        private static string FormatSimple(IEnumerable<Advertisement> values)
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
    }
}
