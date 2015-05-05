using System.Collections.Generic;
using System.Linq;

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
            var positionDto = _positionReadModel.GetPositionSalesModelAndCompositenessByPricePosition(pricePositionId);
            var orderInfo = _orderReadModel.GetOrderInfoToCheckPossibilityOfOrderPositionCreation(orderId);

            if (positionDto.IsComposite && !AtLeastOneLinkingObjectIsSpecifiedForCompositePosition(orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!AtLeastOneLinkingObjectIsSpecified(orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!AllAddressesBelongToFirm(orderInfo.FirmId, orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (!AllPositionsOfTheSameSalesModel(orderPositionId, positionDto.SalesModel, orderInfo.OrderPositions.ToDictionary(x => x.OrderPositionId, x => x.SalesModel), out report))
            {
                return false;
            }

            if (_positionReadModel.KeepCategoriesSynced(positionDto.PositionId) && !CategoriesSetForAllPositionsWithCategoriesMustBeTheSame(orderPositionAdvertisements, out report))
            {
                return false;
            }

            if (_positionReadModel.AllSubpositionsMustBePicked(positionDto.PositionId) && !AllSubpositionsMustBePicked(positionDto.PositionId, orderPositionAdvertisements, out report))
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
        private bool AtLeastOneLinkingObjectIsSpecifiedForCompositePosition(IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
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

        private bool AtLeastOneLinkingObjectIsSpecified(IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
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

        private bool CategoriesSetForAllPositionsWithCategoriesMustBeTheSame(IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                                                             out string report)
        {
            report = null;

            var positionsWithCategories =
                orderPositionAdvertisements.GroupBy(x => x.PositionId)
                                           .ToDictionary(x => x.Key, y => y.Where(z => z.CategoryId.HasValue).Select(z => z.CategoryId.Value))
                                           .Where(x => x.Value.Any());

            var bindingTypes = _positionReadModel.GetPositionBindingObjectTypes(positionsWithCategories.Select(x => x.Key))
                                                 .GroupBy(x => x.Value)
                                                 .ToDictionary(x => x.Key, y => y.Select(z => z.Key));

            foreach (var bindingType in bindingTypes)
            {
                var positionsWithCategoriesToCheck = positionsWithCategories.Where(x => bindingType.Value.Contains(x.Key));

                if (positionsWithCategoriesToCheck.Any(positionWithCategories =>
                                                       positionWithCategories.Value
                                                                             .Any(category =>
                                                                                  positionsWithCategoriesToCheck
                                                                                      .Any(x => x.Key != positionWithCategories.Key && !x.Value.Contains(category)))))
                {
                    var positionNames = _positionReadModel.GetPositionNames(positionsWithCategoriesToCheck.Select(x => x.Key));
                    report = string.Format(BLResources.CategoriesSetForPositionsMustBeTheSame, string.Join(",", positionNames.Select(x => string.Format(@"'{0}'", x.Value))));
                    return false;
                }
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

        private bool AllSubpositionsMustBePicked(long positionId,
                                                 IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                                 out string report)
        {
            report = null;

            var childPositionIds = _positionReadModel.GetChildPositionIds(positionId);

            var pickedPositions = orderPositionAdvertisements.Select(x => x.PositionId).Distinct().ToArray();

            var notPickedPositions = childPositionIds.Where(x => !pickedPositions.Contains(x)).ToArray();

            if (notPickedPositions.Any())
            {
                report = BLResources.AllPositionsMustBePicked;
                return false; 
            }

            return true;
        }

        #endregion
    }
}
