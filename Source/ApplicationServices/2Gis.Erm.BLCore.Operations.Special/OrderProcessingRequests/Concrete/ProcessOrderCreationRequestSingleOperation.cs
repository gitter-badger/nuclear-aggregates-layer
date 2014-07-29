using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

using MessageType = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public class ProcessOrderCreationRequestSingleOperation : IProcessOrderCreationRequestSingleOperation
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IClientDealSelectionService _clientDealSelectionService;
        private readonly IModifyBusinessModelEntityService<Order> _modifyOrderService;
        private readonly IOrderProcessingRequestService _orderProcessingRequestService;
        private readonly IOrderProcessingOwnerSelectionService _ownerSelectionService;        
        private readonly IObtainDealForBizzacountOrderOperationService _obtainDealForBizzacountOrderOperationService;

        public ProcessOrderCreationRequestSingleOperation(IOperationScopeFactory scopeFactory,
                                                          IAccountReadModel accountReadModel,
                                                          IClientDealSelectionService clientDealSelectionService,
                                                          IModifyBusinessModelEntityService<Order> modifyOrderService,
                                                          IOrderProcessingRequestService orderProcessingRequestService,
                                                          IOrderProcessingOwnerSelectionService ownerSelectionService,
                                                          IObtainDealForBizzacountOrderOperationService obtainDealForBizzacountOrderOperationService)
        {
            _orderProcessingRequestService = orderProcessingRequestService;
            _ownerSelectionService = ownerSelectionService;
            _modifyOrderService = modifyOrderService;
            _accountReadModel = accountReadModel;
            _obtainDealForBizzacountOrderOperationService = obtainDealForBizzacountOrderOperationService;
            _clientDealSelectionService = clientDealSelectionService;
            _scopeFactory = scopeFactory;
        }

        public OrderCreationResult ProcessSingle(long requestId)
        {
            var request = _orderProcessingRequestService.GetPrologationRequestToProcess(requestId);
            if (request.RequestType != (int)OrderProcessingRequestType.CreateOrder)
            {
                var message = string.Format(BLResources.OrderCreationRequestTypeMismatch,
                                            (OrderProcessingRequestType)request.RequestType);
                throw new InvalidOperationException(message);
            }

            var processMessages = new List<IMessageWithType>();
            try
            {
                using (var scope = _scopeFactory.CreateNonCoupled<CreateOrderByRequestIdentity>())
                {
                    var orderDto = CreateOrder(request, processMessages);
                    ProcessSuccess(request, processMessages);
                    CompleteOrderProcessingRequest(request, orderDto.Id);

                    scope.Updated<Platform.Model.Entities.Erm.OrderProcessingRequest>(request.Id);
                    scope.Complete();

                    // Дебажные сообщения мы сохраняем, но в окошко на клиент не передаем
                    processMessages =
                        processMessages.Where(x => x.Type != MessageType.Debug && x.MessageText != BLResources.OrderProcessingRequestProcessedSuccessfully)
                                       .ToList();

                    return new OrderCreationResult
                        {
                            Order = new OrderCreationResult.OrderDto { Id = orderDto.Id, Number = orderDto.Number },
                            Messages = processMessages,
                        };
                }
            }
            catch (BusinessLogicException ex)
            {
                // Даже если внешняя транзакция откатится, эта должна быть закоммичена, чтобы не потерять сообщения об ошибках и изменение статуса заявки.
                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                using (var scope = _scopeFactory.CreateNonCoupled<CreateOrderByRequestIdentity>())
                {
                    SetOrderProcessingRequestPending(request);
                    ProcessError(request, ex.Message, processMessages);

                    scope.Updated<Platform.Model.Entities.Erm.OrderProcessingRequest>(request.Id);
                    scope.Complete();
                    transaction.Complete();
                }

                throw;
            }
            catch (Exception)
            {
                // Даже если внешняя транзакция откатится, эта должна быть закоммичена, чтобы не потерять сообщения об ошибках и изменение статуса заявки.
                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                using (var scope = _scopeFactory.CreateNonCoupled<CreateOrderByRequestIdentity>())
                {
                    SetOrderProcessingRequestPending(request);
                    ProcessError(request, BLResources.ApplicationError, processMessages);
                
                    scope.Updated<Platform.Model.Entities.Erm.OrderProcessingRequest>(request.Id);
                    scope.Complete();
                    transaction.Complete();
                }

                throw;
            }
        }

        private OrderProcessingRequestOrderDto CreateOrder(Platform.Model.Entities.Erm.OrderProcessingRequest request, ICollection<IMessageWithType> messages)
        {
            var firmDto = _orderProcessingRequestService.GetFirmDto(request.FirmId);
            if (firmDto == null)
            {
                throw new EntityNotFoundException(typeof(Platform.Model.Entities.Erm.OrderProcessingRequest), request.FirmId);
            }

            var sourceOrganizationUnitId = request.SourceOrganizationUnitId;
            var destOrganizationUnitId = firmDto.OrganizationUnitId;

            if (firmDto.Client == null)
            {
                var message = string.Format(BLResources.OrderCreationRequestNotProcessedClientNotFound, request.Id);
                throw new BusinessLogicException(message);
            }

            // TODO {all, 16.12.2013}: логику работы со сделкой по-хорошему бы вернуть в ObtainDealForBizzacountOrderOperationService, 
            // TODO {all, 16.12.2013}: для этого можно ему в качестве зависимости добавить IOrderProcessingOwnerSelectionService, а отсюда его убрать
            // Получаем существующую сделку
            var dealResult = GetDealForClient(firmDto.Client.Id, firmDto.OwnerCode);
            if (dealResult != null)
            {
                var messageText = string.Format(BLResources.OrderProlongationRequestInformationDealExistingReused, dealResult.DealName);
                messages.Add(new MessageWithType { MessageText = messageText, Type = MessageType.Debug });
            }
            else
            {
                // Определяем куратора сделки на тот случай, если потребуется создать новую (на самом деле это значение может оказаться и не использованным)
                var owner = _ownerSelectionService.FindOwner(request, messages);

            // Получаем существующую или создаём новую сделку для создаваемого заказа
                dealResult = _obtainDealForBizzacountOrderOperationService.CreateDealForClient(firmDto.Client.Id, owner.Id);

                var messageText = string.Format(BLResources.OrderProlongationRequestInformationDealNewCreated, dealResult.DealName);
                messages.Add(new MessageWithType { MessageText = messageText, Type = MessageType.Debug });
            }

            // Получение лицевого счёта через _accountRepository.CreateAccount
            var boou = _accountReadModel.FindPrimaryBranchOfficeOrganizationUnit(destOrganizationUnitId);
            if (boou == null)
            {
                var message = string.Format(BLResources.OrderCreationRequestNotProcessedBranchOrganizationUnitNotFound, destOrganizationUnitId);
                throw new BusinessLogicException(message);
            }

            var account = FindAccount(request.LegalPersonId, boou.Id, messages);

            var order = new OrderDomainEntityDto
                {
                    // Полезные значения
                    FirmRef = new EntityReference(request.FirmId),
                    SourceOrganizationUnitRef = new EntityReference(sourceOrganizationUnitId),
                    DestOrganizationUnitRef = new EntityReference(destOrganizationUnitId),
                    LegalPersonRef = new EntityReference(request.LegalPersonId),
                    BeginDistributionDate = request.BeginDistributionDate,
                    ReleaseCountPlan = request.ReleaseCountPlan,
                    ReleaseCountFact = request.ReleaseCountPlan,
                    OrderType = OrderType.Sale,
                    DealRef = new EntityReference(dealResult.DealId),
                    AccountRef = account != null ? new EntityReference(account.Id) : new EntityReference(),
                    OwnerRef = new EntityReference(dealResult.DealOwnerCode),
                    BranchOfficeOrganizationUnitRef = new EntityReference(boou.Id),
                    CurrencyRef = new EntityReference(firmDto.CurrencyId),
                    HasDocumentsDebt = DocumentsDebt.Absent,
                    LegalPersonProfileRef = new EntityReference(request.LegalPersonProfileId),

                    // EndDistributionDatePlan, EndDistributionDateFact считаются стратегией
                    // BeginReleaseNumber, EndReleaseNumberPlan, EndReleaseNumberFact тоже заполняет стратегия

                    // Служебные значения
                    SignupDate = DateTime.UtcNow,
                    WorkflowStepId = OrderState.OnRegistration,
                    PreviousWorkflowStepId = OrderState.OnRegistration,
                    IsActive = true,
                    IsDeleted = false,

                    // Обязательные значения
                    BargainRef = new EntityReference(),
                    InspectorRef = new EntityReference(),
                    PlatformRef = new EntityReference(),
                };

            var orderId = _modifyOrderService.Modify(order);

            return _orderProcessingRequestService.GetOrderDto(orderId);
        }

        private ObtainDealForBizzacountOrderResult GetDealForClient(long clientId, long ownerCode)
        {
            var openedDeals = _clientDealSelectionService.GetOpenedDeals(clientId).ToArray();

            var deal = openedDeals.Length == 1
                           ? openedDeals.Single()
                           : openedDeals.FirstOrDefault(x => x.OwnerCode == ownerCode);

            return deal != null
                       ? new ObtainDealForBizzacountOrderResult { DealId = deal.Id, DealOwnerCode = deal.OwnerCode, DealName = deal.Name, }
                       : null;
        }

        private Account FindAccount(long legalPersonId, long branchOfficeOrganizationUnitId, ICollection<IMessageWithType> messages)
        {
            var account = _accountReadModel.FindAccount(legalPersonId, branchOfficeOrganizationUnitId);
            var legalPeronName = _accountReadModel.GetLegalPersonShortName(legalPersonId);
            var branchOfficeOrganizationUnitName = _accountReadModel.GetBranchOfficeOrganizationUnitName(branchOfficeOrganizationUnitId);

            var message = account == null
                ? string.Format(BLResources.OrderCreationRequestInformationExistingAccountNotFound, legalPeronName, branchOfficeOrganizationUnitName)
                : string.Format(BLResources.OrderCreationRequestInformationExistingAccountFound, legalPeronName, branchOfficeOrganizationUnitName);

            messages.Add(new MessageWithType { MessageText = message, Type = MessageType.Debug });
            return account;
        }

        private void ProcessSuccess(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, ICollection<IMessageWithType> processingMessages)
        {
            processingMessages.Add(new MessageWithType { MessageText = BLResources.OrderProcessingRequestProcessedSuccessfully, Type = MessageType.Info });

            _orderProcessingRequestService.SaveMessagesForOrderProcessingRequest(orderProcessingRequest.Id, processingMessages);
        }

        private void ProcessError(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest,
                                  string errorText,
                                  ICollection<IMessageWithType> processingMessages)
        {
            processingMessages.Add(new MessageWithType { MessageText = errorText, Type = MessageType.Error });

            _orderProcessingRequestService.SaveMessagesForOrderProcessingRequest(orderProcessingRequest.Id, processingMessages);
        }

        private void CompleteOrderProcessingRequest(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, long createdOrderId)
        {
            orderProcessingRequest.RenewedOrderId = createdOrderId;
            orderProcessingRequest.State = (int)OrderProcessingRequestState.Completed;
            _orderProcessingRequestService.Update(orderProcessingRequest);
        }

        private void SetOrderProcessingRequestPending(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            orderProcessingRequest.State = (int)OrderProcessingRequestState.Pending;
            _orderProcessingRequestService.Update(orderProcessingRequest);
        }
    }
}
