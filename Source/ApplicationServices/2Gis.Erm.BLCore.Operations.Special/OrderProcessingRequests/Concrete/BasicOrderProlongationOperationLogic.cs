using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public sealed class BasicOrderProlongationOperationLogic : IBasicOrderProlongationOperationLogic 
    {
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICopyOrderOperationService _copyOrderOperationService;
        private readonly IRepairOutdatedPositionsOperationService _repairOutdatedPositionsOperationService;
        private readonly IObtainDealForBizzacountOrderOperationService _obtainDealForBizzacountOrderOperationService;
        private readonly IOrderProcessingOwnerSelectionService _orderProcessingOwnerSelectionService;
        private readonly IOrderProcessingRequestEmailSender _emailSender;

        public BasicOrderProlongationOperationLogic(
            IOrderProcessingRequestService orderProcessingRequestService,
            IOperationScopeFactory scopeFactory,
            ICopyOrderOperationService copyOrderOperationService,
            IRepairOutdatedPositionsOperationService repairOutdatedPositionsOperationService,
            IObtainDealForBizzacountOrderOperationService obtainDealForBizzacountOrderOperationService,
            IOrderProcessingOwnerSelectionService orderProcessingOwnerSelectionService,
            IOrderProcessingRequestEmailSender emailSender)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
            _scopeFactory = scopeFactory;
            _copyOrderOperationService = copyOrderOperationService;
            _repairOutdatedPositionsOperationService = repairOutdatedPositionsOperationService;
            _obtainDealForBizzacountOrderOperationService = obtainDealForBizzacountOrderOperationService;
            _orderProcessingOwnerSelectionService = orderProcessingOwnerSelectionService;
            _emailSender = emailSender;
        }

        public OrderProcessingResult ExecuteRequest(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            if (orderProcessingRequest.RequestType != OrderProcessingRequestType.ProlongateOrder)
            {
                var message = string.Format(BLResources.OrderProlongationRequestTypeMismatch,
                                            orderProcessingRequest.RequestType);
                throw new InvalidOperationException(message);
            }

            var messages = new List<IMessageWithType>();
            try
            {
                if (orderProcessingRequest.BaseOrderId == null)
                {
                    throw new InvalidOperationException(BLResources.OrderForProlongationNotFound);
                }

                if (orderProcessingRequest.BeginDistributionDate < DateTime.Today)
                {
                    throw new BusinessLogicException(BLResources.BeginDateMustNotBeInPast);
                }

                using (var scope = _scopeFactory.CreateNonCoupled<ProlongateOrderByRequestIdentity>())
                {
                    // TODO {all, 16.12.2013}: вызов этого метода можно запихнуть в _obtainDealForBizzacountOrderOperationService.ObtainDealForOrder
                    var owner = _orderProcessingOwnerSelectionService.FindOwner(orderProcessingRequest, messages);

                    var dealId = _obtainDealForBizzacountOrderOperationService.ObtainDealForOrder(orderProcessingRequest.BaseOrderId.Value, owner.Id).DealId;

                    var copyOrderResponse = _copyOrderOperationService.CopyOrder(orderProcessingRequest.BaseOrderId.Value,
                                                                                 orderProcessingRequest.BeginDistributionDate,
                                                                                 orderProcessingRequest.ReleaseCountPlan,
                                                                                 DiscountType.InPercents,
                                                                                 dealId);

                    var repairErrors = _repairOutdatedPositionsOperationService.RepairOutdatedPositions(copyOrderResponse.OrderId, true).AsArray();
                    messages.AddRange(repairErrors);
                    CompleteOrderProcessingRequest(orderProcessingRequest, copyOrderResponse);
                    messages.Add(GetSuccessMessage());

                    var emailSendingResult = _emailSender.SendProcessingMessages(orderProcessingRequest, messages);
                    messages.AddRange(GetEmailSendingErrors(emailSendingResult.Errors));
                    _orderProcessingRequestService.SaveMessagesForOrderProcessingRequest(orderProcessingRequest.Id, messages.ToArray());

                    scope.Updated<Platform.Model.Entities.Erm.OrderProcessingRequest>(orderProcessingRequest.Id)
                         .Complete();

                    return new OrderProcessingResult
                        {
                            OrderId = copyOrderResponse.OrderId,
                            OrderNumber = copyOrderResponse.OrderNumber,
                            Messages = messages.OfType<RepairOutdatedPositionsOperationMessage>().Where(x => x.Type != MessageType.Debug)
                        };
                }
            }
            catch (BusinessLogicException ex)
            {
                // Даже если внешняя транзакция откатится, эта должна быть закоммичена, чтобы не потерять сообщения об ошибках и изменение статуса заявки.
                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                using (var scope = _scopeFactory.CreateNonCoupled<ProlongateOrderByRequestIdentity>())
                {
                    ProcessError(orderProcessingRequest, messages, ex.Message);
                    SetOrderProcessingRequestPending(orderProcessingRequest);

                    scope.Updated<Platform.Model.Entities.Erm.OrderProcessingRequest>(orderProcessingRequest.Id);
                    scope.Complete();
                    transaction.Complete();
                }

                throw;
            }
            catch (Exception)
            {
                // Даже если внешняя транзакция откатится, эта должна быть закоммичена, чтобы не потерять сообщения об ошибках и изменение статуса заявки.
                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                using (var scope = _scopeFactory.CreateNonCoupled<ProlongateOrderByRequestIdentity>())
                {
                    ProcessError(orderProcessingRequest, messages, BLResources.ApplicationError);
                    SetOrderProcessingRequestPending(orderProcessingRequest);

                    scope.Updated<Platform.Model.Entities.Erm.OrderProcessingRequest>(orderProcessingRequest.Id);
                    scope.Complete();
                    transaction.Complete();
                }

                throw;
            }
        }

        private static IMessageWithType GetSuccessMessage()
        {
            return new MessageWithType { MessageText = BLResources.OrderProcessingRequestProcessedSuccessfully, Type = MessageType.Info };
        }

        private static IEnumerable<IMessageWithType> GetEmailSendingErrors(IEnumerable<string> errors)
        {
            return errors.Select(x => new MessageWithType
            {
                MessageText = x,
                Type = MessageType.Debug
            });
        }

        private void ProcessError(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, List<IMessageWithType> processingMessages, string errorText)
        {
            var errorMessage = new MessageWithType { MessageText = errorText, Type = MessageType.Error, };

            var emailSendingResult = _emailSender.SendProcessingMessages(orderProcessingRequest, new[] { errorMessage });
            
            processingMessages.Add(errorMessage);
            processingMessages.AddRange(GetEmailSendingErrors(emailSendingResult.Errors));

            _orderProcessingRequestService.SaveMessagesForOrderProcessingRequest(orderProcessingRequest.Id, processingMessages);
        }

        private void CompleteOrderProcessingRequest(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, CopyOrderResult copyOrderResponse)
        {
            orderProcessingRequest.RenewedOrderId = copyOrderResponse.OrderId;
            orderProcessingRequest.State = OrderProcessingRequestState.Completed;
            _orderProcessingRequestService.Update(orderProcessingRequest);
        }

        private void SetOrderProcessingRequestPending(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            orderProcessingRequest.State = OrderProcessingRequestState.Pending;
            _orderProcessingRequestService.Update(orderProcessingRequest);
        }
    }
}
