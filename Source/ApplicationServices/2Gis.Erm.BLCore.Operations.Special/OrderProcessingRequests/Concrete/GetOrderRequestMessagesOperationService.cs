using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // TODO {all, 04.02.2014}: не понятно какую ценность представляет такой OperationService с точки зрения бизнес домена ERM - скорее всего должен быть сконвертирован в ReadModel
    public class GetOrderRequestMessagesOperationService : IGetOrderRequestMessagesOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IReadOrderRequestMessagesService _readOrderRequestMessagesService;

        public GetOrderRequestMessagesOperationService(IOperationScopeFactory operationScopeFactory,
                                                       IReadOrderRequestMessagesService readOrderRequestMessagesService)
        {
            _operationScopeFactory = operationScopeFactory;
            _readOrderRequestMessagesService = readOrderRequestMessagesService;
        }

        public IEnumerable<RequestMessageDetailDto> GetRequestMessages(long requestId)
        {
            IEnumerable<RequestMessageDetailDto> messages;
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetOrderRequestMessagesIdentity>())
            {
                messages = _readOrderRequestMessagesService.GetRequestMessages(requestId);
                scope.Complete();
            }

            return messages;
        }
    }
}
