using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // FIXME {all, 13.11.2013}: используется Finder, что не допустимо для OperationService
    public class CancelOrderProcessingRequestOperationService : ICancelOrderProcessingRequestOperationService
    {
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IFinder _finder;
        private readonly IOperationScopeFactory _scopeFactory;

        public CancelOrderProcessingRequestOperationService(
            IOrderProcessingRequestService orderProcessingRequestService,
            IFinder finder,
            IOperationScopeFactory scopeFactory)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
            _finder = finder;
            _scopeFactory = scopeFactory;
        }

        public void CancelRequest(long requestId)
        {
            var orderProcessingRequest = _finder.FindObsolete(Specs.Find.ById<OrderProcessingRequest>(requestId)).Single();
            if (orderProcessingRequest.State == OrderProcessingRequestState.Completed)
            {
                throw new BusinessLogicException(BLResources.CannotCancelCompletedOrderRequest);
            }

            if (orderProcessingRequest.State == OrderProcessingRequestState.Cancelled)
            {
                throw new BusinessLogicException(BLResources.CannotCancelCanceledOrderRequest);
            }

            using (var scope = _scopeFactory.CreateNonCoupled<CancelOrderProcessingRequestIdentity>())
            {
                orderProcessingRequest.State = OrderProcessingRequestState.Cancelled;
                _orderProcessingRequestService.Update(orderProcessingRequest);

                var message = new MessageWithType
                    {
                        MessageText = BLResources.OrderProcessingRequestWasCancelled,
                        Type = MessageType.Info
                    };

                _orderProcessingRequestService.SaveMessagesForOrderProcessingRequest(requestId, new IMessageWithType[] { message });

                scope.Updated<OrderProcessingRequest>(requestId);

                scope.Complete();
            }
        }
    }
}

