using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public sealed class CanChangeOrderPositionBindingObjectsDetector : ICanChangeOrderPositionBindingObjectsDetector
    {
        // элементы привязки, которые можно менять.
        // согласно http://confluence.dvlp.2gis.local/pages/viewpage.action?pageId=95750732
        private static readonly PositionBindingObjectType[] AllowedPositionTypes =
            {
                PositionBindingObjectType.AddressSingle,
                PositionBindingObjectType.AddressMultiple,
                PositionBindingObjectType.CategorySingle,
                PositionBindingObjectType.CategoryMultiple,
                PositionBindingObjectType.CategoryMultipleAsterix,
                PositionBindingObjectType.AddressCategorySingle,
                PositionBindingObjectType.AddressCategoryMultiple,
                PositionBindingObjectType.AddressFirstLevelCategorySingle,
                PositionBindingObjectType.AddressFirstLevelCategoryMultiple,
            };

        // Состояния заказа, в котором можно менять объекты привязки
        private static readonly OrderState[] AllowedOrderStates =
            {
                OrderState.Approved,
            };

        public bool CanChange(
            OrderState orderWorkflowState, 
            PositionBindingObjectType orderPositionBindingObject, 
            bool skipAdvertisementCountCheck,
            int? actualOrderPositionAdvertisementLinksCount,
            int? targetOrderPositionAdvertisementLinksCount,
            out string report)
        {
            if (!skipAdvertisementCountCheck)
            {
                if (!actualOrderPositionAdvertisementLinksCount.HasValue || !targetOrderPositionAdvertisementLinksCount.HasValue)
                {
                    throw new ArgumentException("check actualOrderPositionAdvertisementLinksCount or targetOrderPositionAdvertisementLinksCount args");
                }

                if (actualOrderPositionAdvertisementLinksCount.Value != targetOrderPositionAdvertisementLinksCount.Value)
                {
                    report = string.Format(BLResources.InvalidBindingObjectCount, actualOrderPositionAdvertisementLinksCount, targetOrderPositionAdvertisementLinksCount);
                    return false;
                }
            }

            if (!AllowedPositionTypes.Contains(orderPositionBindingObject))
            {
                report = string.Format(BLResources.InvalidOrderPositionBindingType,
                                                                     orderPositionBindingObject,
                                                                     string.Join(", ", AllowedPositionTypes));
                return false;
            }

            if (!AllowedOrderStates.Contains(orderWorkflowState))
            {
                report = string.Format(BLResources.InvalidOrderWorkflowSate,
                                                                     orderWorkflowState,
                                                                     string.Join(", ", AllowedOrderStates));

                return false;
            }

            report = null;
            return true;
        }
    }
}