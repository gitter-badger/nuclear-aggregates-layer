using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions
{
    public static class PositionBindingObjectTypeExtensions
    {
        public static bool IsPositionBindingOfSingleType(this PositionBindingObjectType type)
        {
            switch (type)
            {
                case PositionBindingObjectType.Firm:
                case PositionBindingObjectType.AddressCategorySingle:
                case PositionBindingObjectType.AddressSingle:
                case PositionBindingObjectType.CategorySingle:
                case PositionBindingObjectType.AddressFirstLevelCategorySingle:
                    return true;
                case PositionBindingObjectType.AddressMultiple:
                case PositionBindingObjectType.CategoryMultiple:
                case PositionBindingObjectType.CategoryMultipleAsterix:
                case PositionBindingObjectType.AddressCategoryMultiple:
                case PositionBindingObjectType.AddressFirstLevelCategoryMultiple:
                case PositionBindingObjectType.ThemeMultiple:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
