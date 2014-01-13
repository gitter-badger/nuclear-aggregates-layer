using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // TODO {d.ivanov, 05.12.2013}: 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
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
