using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public sealed class CheckIfOrderPositionCanBeModifiedOperationService : ICheckIfOrderPositionCanBeModifiedOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public CheckIfOrderPositionCanBeModifiedOperationService(
            IOrderReadModel orderReadModel,
            IPositionReadModel positionReadModel,
            IFirmReadModel firmReadModel)
        {
            _orderReadModel = orderReadModel;
            _positionReadModel = positionReadModel;
            _firmReadModel = firmReadModel;
        }

        public bool Check(long orderId, long orderPositionId, long pricePositionId, IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements, out string report)
        {
            var position = _positionReadModel.GetPositionByPricePositionId(pricePositionId);
            var orderInfo = _orderReadModel.GetOrderInfoToCheckPossibilityOfOrderPositionCreation(orderId);

            if (position.IsComposite && !AllRequiredAdvertisementsAreSpecifiedForCompositePosition(orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!AllRequiredAdvertisementsAreSpecified(orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!AllAddressesBelongToFirm(orderInfo.FirmId, orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!AllPositionsOfTheSameSalesModel(orderPositionId, position.SalesModel, orderInfo.OrderPositions.ToDictionary(x => x.OrderPositionId, x => x.SalesModel), out report))
            {
                return false;
            }

            if (!CategoriesSetForAllPositionsWithCategoriesMustBeTheSame(position.SalesModel, orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!BindingOfSingleTypeMustHaveNoMoreThan1BindingPerPosition(orderPositionAdvertisements, out report))
            {
                return false;
            }

            // Соответствие рубрики фирме заказа НЕ проверяем, т.к. есть функционал "Добавить рубрику" и ответственность за некорректную рубрику лежит на менеджере
            return true;
        }

        #region Проверки
        // Является частным случаем проверки AllRequiredAdvertisementsAreSpecified, которая появилась в рамках фикса бага ERM-6151. Есть опасение, что новая общая проверка ломает какой-то кейс. 
        // Если жалоб на нее не будет, то эту частную проверку (AllRequiredAdvertisementsAreSpecifiedForCompositePosition) можно будет удалить
        private bool AllRequiredAdvertisementsAreSpecifiedForCompositePosition(IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                              out string report)
        {
            report = null;
            if (!orderPositionAdvertisements.Any())
            {
                report = BLResources.NeedToPickAtLeastOneLinkingObjectForCompositePosition;
                return false;
            }

            return true;
        }

        private bool AllRequiredAdvertisementsAreSpecified(IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                                           out string report)
        {
            report = null;
            if (!orderPositionAdvertisements.Any())
            {
                report = BLResources.NeedToPickAtLeastOneLinkingObject;
                return false;
            }

            return true;
        }

        private bool AllAddressesBelongToFirm(long firmId,
                                              IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                              out string report)
        {
            report = null;

            var distinctOrderPositionFirmAddressIds = orderPositionAdvertisements
                .Where(x => x.FirmAddressId.HasValue)
                .Select(x => x.FirmAddressId.Value)
                .Distinct()
                .ToArray();

            var invalidAddresses = _firmReadModel.GetAddressesNamesWhichNotBelongToFirm(firmId, distinctOrderPositionFirmAddressIds);

            if (invalidAddresses.Any())
            {
                var firmName = _firmReadModel.GetFirmName(firmId);

                var addressNamesDescription = string.Join(", ", invalidAddresses.Select(x => string.Format(@"""{0}""", x)));

                report = string.Format(BLResources.AddressesNotBelongToFirm, addressNamesDescription, firmName);
                return false;
            }

            return true;
        }

        private bool AllPositionsOfTheSameSalesModel(long orderPositionId,
                                                     SalesModel salesModelOfEditingOrderPosition,
                                                     IEnumerable<KeyValuePair<long, SalesModel>> saleModelsOfOrderPositions,
                                                     out string report)
        {
            report = null;
            var orderPositionsWithAnotherSalesModel = saleModelsOfOrderPositions.Where(x => x.Value != salesModelOfEditingOrderPosition).ToArray();
            if (orderPositionsWithAnotherSalesModel.Any() && (orderPositionsWithAnotherSalesModel.Count() > 1 || orderPositionsWithAnotherSalesModel.Single().Key != orderPositionId))
            {
                report = BLResources.CantAddOrderPositionWithSalesModelDifferentFromAnotherOrderPositionsSalesModel;
                return false;
            }

            return true;
        }

        private bool CategoriesSetForAllPositionsWithCategoriesMustBeTheSame(SalesModel salesModelOfEditingOrderPosition,
                                                                             IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                                                             out string report)
        {
            report = null;
            if (!salesModelOfEditingOrderPosition.IsPlannedProvisionSalesModel())
            {
                return true;
            }

            var positionsWithCategories =
                orderPositionAdvertisements.GroupBy(x => x.PositionId)
                                           .ToDictionary(x => x.Key, y => y.Where(z => z.CategoryId.HasValue).Select(z => z.CategoryId.Value))
                                           .Where(x => x.Value.Any());

            if (positionsWithCategories.Any(positionWithCategories =>
                                            positionWithCategories.Value
                                                                  .Any(category =>
                                                                       positionsWithCategories
                                                                           .Any(x => x.Key != positionWithCategories.Key && !x.Value.Contains(category)))))
            {
                report = BLResources.CategoriesSetForAllPositionsMustBeTheSame;
                return false;
            }

            return true;
        }

        private bool BindingOfSingleTypeMustHaveNoMoreThan1BindingPerPosition(IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                                                              out string report)
        {
            var positionIdsToCheck = orderPositionAdvertisements.GroupBy(x => x.PositionId).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            var invalidPositionIds =
                _positionReadModel.GetPositionBindingObjectTypes(positionIdsToCheck)
                                  .Where(x => x.Value.IsPositionBindingOfSingleType())
                                  .Select(x => x.Key)
                                  .ToArray();

            if (invalidPositionIds.Any())
            {
                var invalidPositionNames = _positionReadModel.GetPositionNames(invalidPositionIds);
                report = string.Format(BLResources.CannotPickMoreThanOneLinkingObject, string.Join(",", invalidPositionNames.Values));

                return false;
            }

            report = null;
            return true;
        }

        #endregion
    }
}
