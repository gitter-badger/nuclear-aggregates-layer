using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class ChangeBindingObjectsHandler : RequestHandler<ChangeBindingObjectsRequest, EmptyResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;

        public ChangeBindingObjectsHandler(IUnitOfWork unitOfWork, 
            ISubRequestProcessor requestProcessor,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService)
        {
            _unitOfWork = unitOfWork;
            _requestProcessor = requestProcessor;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
        }

        // TODO {all, 09.09.2013}: Скорее всего, нужно логировать как отдельную операцию. Сейчас логируется, как удаление и создание OrderPositionAdvertisement
        protected override EmptyResponse Handle(ChangeBindingObjectsRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            using (var scope = _unitOfWork.CreateScope())
            {
                var checkResponse = (CheckIsBindingObjectChangeAllowedResponse)
                    _requestProcessor.HandleSubRequest(new CheckIsBindingObjectChangeAllowedRequest
                        {
                            AdvertisementCount = request.Advertisements.Length, 
                            OrderPositionId = request.OrderPositionId, 
                        }, Context);

                if(!checkResponse.IsChangeAllowed)
                    throw new NotificationException(checkResponse.ErrorMessage);

                var repo = scope.CreateRepository<IOrderRepository>();
                repo.ChangeOrderPositionBindingObjects(request.OrderPositionId, request.Advertisements);

                _registerOrderStateChangesOperationService.Changed(checkResponse.OrderId,
                                                                   OrderValidationRuleGroup.Generic,
                                                                   OrderValidationRuleGroup.AdvertisementMaterialsValidation,
                                                                   OrderValidationRuleGroup.ADPositionsValidation);
                
                scope.Complete();
                transaction.Complete();
                return Response.Empty;
            }
        }
    }
}