using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete.Simplified
{
    // 2+  \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
    // FIXME {all, 13.11.2013}: размыта ответсвенность этого типа - часть функционала по работе с OrderProcessingRequest, напрямую использует DAL (что запрещено), часть через simplifiedmodelconumer - итого нет четкого единого подхода при рефакторинге ApplicationServices - будут доп. проблемы из-за этого
    public class OrderProcessingRequestService : IOrderProcessingRequestService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<OrderProcessingRequest> _orderProcessingRequestRepository;
        private readonly IRepository<OrderProcessingRequestMessage> _orderProcessingMessageRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IFinder _finder;

        public OrderProcessingRequestService(
            IOperationScopeFactory scopeFactory,
            IRepository<OrderProcessingRequest> orderProcessingRequestRepository,
            IRepository<OrderProcessingRequestMessage> orderProcessingMessageRepository,
            IIdentityProvider identityProvider,
            IFinder finder)
        {
            _scopeFactory = scopeFactory;
            _orderProcessingRequestRepository = orderProcessingRequestRepository;
            _orderProcessingMessageRepository = orderProcessingMessageRepository;
            _identityProvider = identityProvider;
            _finder = finder;
        }

        public void Update(OrderProcessingRequest orderProcessingRequest)
        {
            if (orderProcessingRequest.IsNew())
            {
                throw new ArgumentException(BLResources.MustBeExistingEntity, "orderProcessingRequest");
            }

            _orderProcessingRequestRepository.Update(orderProcessingRequest);
            _orderProcessingRequestRepository.Save();
        }

        public void Create(OrderProcessingRequest orderProcessingRequest)
        {
            if (!orderProcessingRequest.IsNew())
            {
                throw new ArgumentException(BLResources.MustBeNewEntity, "orderProcessingRequest");
            }

            _identityProvider.SetFor(orderProcessingRequest);
            _orderProcessingRequestRepository.Add(orderProcessingRequest);
            _orderProcessingRequestRepository.Save();
        }

        public void SaveMessagesForOrderProcessingRequest(long orderProcessingRequestId, IEnumerable<IMessageWithType> messages)
        {
            var groupId = Guid.NewGuid();

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.OrderProcessingRequestMessage))
            {
                var orderProcessingResuestMessages = messages
                    .Select(x => x.ToOrderProcessingRequestMessage(orderProcessingRequestId))
                    .ToArray();

                foreach (var message in orderProcessingResuestMessages)
                {
                    message.GroupId = groupId;
                    _identityProvider.SetFor(message);
                    _orderProcessingMessageRepository.Add(message);
                }

                _orderProcessingMessageRepository.Save();

                scope.Added<OrderProcessingRequestMessage>(orderProcessingResuestMessages.Select(x => x.Id).ToArray())
                     .Complete();
            }
        }

        public IEnumerable<OrderProcessingRequest> GetPrologationRequestsToProcess()
        {
            return _finder.Find(OrderProcessingRequestSpecifications.Find.ForProlongateAndOpened()).ToArray();
        }

        public OrderProcessingRequest GetPrologationRequestToProcess(long id)
        {
            return _finder.Find(Specs.Find.ById<OrderProcessingRequest>(id)).Single();
        }

        public long GetBaseOrderOwner(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.OwnerCode).Single();
        }

        public OrderProcessingRequestFirmDto GetFirmDto(long firmId)
        {
            return _finder.Find(Specs.Find.ById<Firm>(firmId))
                                   .Select(firm => new OrderProcessingRequestFirmDto
                                       {
                                           OrganizationUnitId = firm.OrganizationUnit.Id,
                                           OrganizationUnitName = firm.OrganizationUnit.Name,
                                           CurrencyId = firm.OrganizationUnit.Country.CurrencyId,
                                           OwnerCode = firm.OwnerCode,
                                           
                                           // Хитрость: мы не можем написать firm.Client == null ? null : ...
                                           Client = new[] { firm.Client }.Where(x => x != null)
                                           .Select(x => new OrderProcessingRequestFirmDto.ClientDto { Id = x.Id, OwnerCode = x.OwnerCode, Name = x.Name }).FirstOrDefault()
                                       })
                                   .SingleOrDefault();
        }

        public OrderProcessingRequestOrderDto GetOrderDto(long orderId)
        {
            // TODO {a.rechkalov, 05.12.2013}: тут можно использовать select-спецификацию
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new OrderProcessingRequestOrderDto { Id = order.Id, Number = order.Number, OwnerCode = order.OwnerCode })
                          .SingleOrDefault();
        }

        public OrderProcessingRequestNotificationData GetNotificationData(long orderProcessingRequestId)
        {
            return _finder.Find(Specs.Find.ById<OrderProcessingRequest>(orderProcessingRequestId))
                          .Select(x => new OrderProcessingRequestNotificationData
                              {
                                  FirmName = x.Firm.Name,
                                  LegalPersonName = x.LegalPerson.LegalName,
                                  BaseOrderNumber = x.BaseOrder.Number,
                                  RenewedOrderNumber = x.RenewedOrder.Number
                              }).Single();
        }
    }
}
