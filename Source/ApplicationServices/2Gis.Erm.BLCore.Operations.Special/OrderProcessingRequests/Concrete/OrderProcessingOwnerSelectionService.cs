using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Model.Metadata.Operations.Identity.Specific.OrderProcessingRequest;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
    public class OrderProcessingOwnerSelectionService : IOrderProcessingOwnerSelectionService
    {
        private readonly IOrderProcessingRequestOwnerSelectionService _userSelectionService;
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderProcessingOwnerSelectionService(
            IOrderProcessingRequestOwnerSelectionService userSelectionService,
            IOrderProcessingRequestService orderProcessingRequestService,
            IOperationScopeFactory scopeFactory)
        {
            _userSelectionService = userSelectionService;
            _orderProcessingRequestService = orderProcessingRequestService;
            _scopeFactory = scopeFactory;
        }

        public User FindOwner(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, ICollection<IMessageWithType> messages)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<SelectOrderProcessingOwnerIdentity>())
            {
                var result = GetBaseOrderOwner(orderProcessingRequest, messages)
                       ?? GetFirmClientOwner(orderProcessingRequest, messages)
                       ?? GetFirmOrganizationUnitDirector(orderProcessingRequest, messages)
                       ?? GetReserveUser(messages);

                scope.Complete();

                return result;
            }
        }

        private User GetBaseOrderOwner(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, ICollection<IMessageWithType> messages)
        {
            if (orderProcessingRequest.RequestType != (int)OrderProcessingRequestType.ProlongateOrder)
            {
                return null;
            }

            if (orderProcessingRequest.BaseOrderId == null)
            {
                throw new InvalidOperationException(BLResources.OrderForProlongationNotFound);
            }

            var baseOrderId = orderProcessingRequest.BaseOrderId.Value;
            var baseOrder = _orderProcessingRequestService.GetOrderDto(baseOrderId);

            return Strategy(() => _userSelectionService.GetOwner(baseOrder.OwnerCode),
                            () => string.Format(BLResources.OrderProlongationRequestInformationBaseOrderOwnerNotFound, baseOrder.Number),
                            () => string.Format(BLResources.OrderProlongationRequestInformationBaseOrderOwnerFound, baseOrder.Number),
                            messages);
        }

        private User GetFirmClientOwner(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, ICollection<IMessageWithType> messages)
        {
            var firmDto = _orderProcessingRequestService.GetFirmDto(orderProcessingRequest.FirmId);
            if (firmDto == null)
            {
                throw new EntityNotFoundException(typeof(Platform.Model.Entities.Erm.OrderProcessingRequest), orderProcessingRequest.FirmId);
            }

            if (firmDto.Client == null)
            {
                var message = string.Format(BLResources.OrderCreationRequestNotProcessedClientNotFound, orderProcessingRequest.Id);
                throw new BusinessLogicException(message);
            }

            return Strategy(() => _userSelectionService.GetOwner(firmDto.Client.OwnerCode),
                            () => string.Format(BLResources.OrderCreationRequestInformationClientOwnerNotFound, firmDto.Client.Name),
                            () => string.Format(BLResources.OrderCreationRequestInformationClientOwnerFound, firmDto.Client.Name),
                            messages);
        }

        private User GetFirmOrganizationUnitDirector(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, ICollection<IMessageWithType> messages)
        {
            var firmDto = _orderProcessingRequestService.GetFirmDto(orderProcessingRequest.FirmId);
            if (firmDto == null)
            {
                throw new EntityNotFoundException(typeof(Platform.Model.Entities.Erm.OrderProcessingRequest), orderProcessingRequest.FirmId);
            }

            return Strategy(() => _userSelectionService.GetOrganizationUnitDirector(firmDto.OrganizationUnitId),
                            () => string.Format(BLResources.OrderCreationRequestInformationOrganizationUnitDirectorNotFound, firmDto.OrganizationUnitName),
                            () => string.Format(BLResources.OrderCreationRequestInformationOrganizationUnitDirectorFound, firmDto.OrganizationUnitName),
                            messages);
        }

        private User GetReserveUser(ICollection<IMessageWithType> messages)
        {
            return Strategy(() => _userSelectionService.GetReserveUser(),
                            () => string.Format(BLResources.OrderCreationRequestInformationReserveUserNotFound),
                            () => string.Format(BLResources.OrderCreationRequestInformationReserveUserFound),
                            messages);
        }

        private static T Strategy<T>(Func<T> entityObtainer,
                                     Func<string> failMessageGenerator,
                                     Func<string> successMessageGenerator,
                                     ICollection<IMessageWithType> messages)
            where T : class
        {
            var entity = entityObtainer.Invoke();
            var message = entity == null
                              ? failMessageGenerator.Invoke()
                              : successMessageGenerator.Invoke();

            messages.Add(new MessageWithType { MessageText = message, Type = MessageType.Debug });
            return entity;
        }
    }
}