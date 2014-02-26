using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class CanCreateOrderPositionForOrderHandler : RequestHandler<CanCreateOrderPositionForOrderRequest, CanCreateOrderPositionForOrderResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IFirmRepository _firmRepository;

        public CanCreateOrderPositionForOrderHandler(
            IFirmRepository firmRepository,
            IOrderReadModel orderReadModel)
        {

            _orderReadModel = orderReadModel;
            _firmRepository = firmRepository;
        }

        protected override CanCreateOrderPositionForOrderResponse Handle(CanCreateOrderPositionForOrderRequest request)
        {
            var response = new CanCreateOrderPositionForOrderResponse();
            var completionState = _orderReadModel.GetOrderCompletionState(request.OrderId);
            if (!completionState.BranchOfficeOrganizationUnit || !completionState.LegalPerson)
            {
                response.Message = BuildOrderNotCompletedMessage(completionState);
                return response;
            }

            var orderType = _orderReadModel.GetOrderType(request.OrderId);
            if (orderType != request.OrderType)
            {
                response.Message = BLResources.OrderTypeHasBeenChanged;
                return response;
            }

            if (request.IsPositionComposite && request.AdvertisementLinksCount == 0)
            {
                response.Message = BLResources.NeedToPickAtLeastOneLinkingObjectForCompositePosition;
                return response;
            }

            if (request.FirmId.HasValue && request.OrderPositionCategoryIds != null && request.OrderPositionFirmAddressIds != null)
            {
                var addressIdsOfTheOrderFirm = _firmRepository.GetFirmAddressesIds(request.FirmId.Value);

                var distinctOrderPositionFirmAddressIds = request.OrderPositionFirmAddressIds.Distinct().ToArray();

                // Проверяем соответствие адреса фирме заказа
                var invalidAddressIds = distinctOrderPositionFirmAddressIds.Where(x => !addressIdsOfTheOrderFirm.Contains(x)).ToArray();

                if (invalidAddressIds.Any())
                {
                    var invalidAddressNames = _firmRepository.GetAddressesNames(invalidAddressIds);

                    var firm = _firmRepository.GetFirm(request.FirmId.Value);

                    var addressNamesDescription = string.Join(", ", invalidAddressNames.Select(x => string.Format(@"""{0}""", x)));

                    response.Message = string.Format(BLResources.AddressesNotBelongToFirm, addressNamesDescription, firm.Name);
                    return response;
                }

                // Соответствие рубрики фирме заказа НЕ проверяем, т.к. есть функционал "Добавить рубрику" и ответственность за некорректную рубрику лежит на менеджере
            }

            return response;
        }

        private static string BuildOrderNotCompletedMessage(OrderCompletionState completionState)
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

            return string.Format(BLResources.OrderFieldsMustBeSpecified, string.Join(", ", fields));
        }
    }
}
