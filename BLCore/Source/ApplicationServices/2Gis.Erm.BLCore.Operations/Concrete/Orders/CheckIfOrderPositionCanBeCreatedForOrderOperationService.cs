using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public sealed class CheckIfOrderPositionCanBeCreatedForOrderOperationService : ICheckIfOrderPositionCanBeCreatedForOrderOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public CheckIfOrderPositionCanBeCreatedForOrderOperationService(
            IOrderReadModel orderReadModel,
            IPositionReadModel positionReadModel,
            IFirmReadModel firmReadModel)
        {
            _orderReadModel = orderReadModel;
            _positionReadModel = positionReadModel;
            _firmReadModel = firmReadModel;
        }

        public bool Check(long orderId, OrderType orderType, out string report)
        {
            if (!AreLegalPersonsSpecified(orderId, out report))
            {
                return false;
            }

            if (!IsOrderTypeCorrect(orderId, orderType, out report))
            {
                return false;
            }

            return true;
        }

        public bool Check(long orderId, long orderPositionId, long pricePositionId, IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements, out string report)
        {
            var position = _positionReadModel.GetPositionByPricePositionId(pricePositionId);
            var orderInfo = _orderReadModel.GetOrderInfoToCheckPossibilityOfOrderPositionCreation(orderId);

            if (!AllRequiredAdvertisementsAreSpecified(position.IsComposite, orderPositionAdvertisements, out report))
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

            // Соответствие рубрики фирме заказа НЕ проверяем, т.к. есть функционал "Добавить рубрику" и ответственность за некорректную рубрику лежит на менеджере
            return true;
        }

        #region Проверки
        private bool AreLegalPersonsSpecified(long orderId,
                                              out string report)
        {
            report = null;
            var completionState = _orderReadModel.GetOrderCompletionState(orderId);
            if (!completionState.BranchOfficeOrganizationUnit || !completionState.LegalPerson)
            {
                var fields = new List<string>();

                if (!completionState.LegalPerson)
                {
                    fields.Add(BLResources.LegalPersonOrderFieldName);
                }

                if (!completionState.BranchOfficeOrganizationUnit)
                {
                    fields.Add(BLResources.BranchOfficeOrganizationUnitOrderFieldName);
                }

                report = string.Format(BLResources.OrderFieldsMustBeSpecified, string.Join(", ", fields));
                return false;
            }

            return true;
        }

        private bool AllRequiredAdvertisementsAreSpecified(bool isCompositePosition,
            IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements,
                                              out string report)
        {
            report = null;
            if (isCompositePosition && !orderPositionAdvertisements.Any())
            {
                report = BLResources.NeedToPickAtLeastOneLinkingObjectForCompositePosition;
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

        private bool IsOrderTypeCorrect(long orderId,
                                        OrderType orderType,
                                        out string report)
        {
            report = null;
            var currentOrderType = _orderReadModel.GetOrderType(orderId);
            if (orderType != currentOrderType)
            {
                report = BLResources.OrderTypeHasBeenChanged;
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

        #endregion
    }
}
