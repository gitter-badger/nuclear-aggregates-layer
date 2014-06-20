using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals
{
    public class DealRepository : IDealRepository
    {
        private readonly ISecureFinder _finder;
        private readonly ISecureRepository<Deal> _dealGenericSecureRepository;
        private readonly ISecureRepository<Order> _orderGenericRepository;
        private readonly ISecureRepository<OrderPosition> _orderPositionGenericRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DealRepository(
            ISecureFinder finder,
            ISecureRepository<Deal> dealGenericSecureRepository,
            ISecureRepository<Order> orderGenericRepository,
            ISecureRepository<OrderPosition> orderPositionGenericRepository,
            ISecurityServiceEntityAccess entityAccessService,
            IIdentityProvider identityProvider, 
            IOperationScopeFactory operationScopeFactory)
        {
            _finder = finder;
            _dealGenericSecureRepository = dealGenericSecureRepository;
            _orderGenericRepository = orderGenericRepository;
            _orderPositionGenericRepository = orderPositionGenericRepository;
            _entityAccessService = entityAccessService;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Update(Deal deal)
        {
            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(deal))
            {
                _dealGenericSecureRepository.Update(deal);
                _dealGenericSecureRepository.Save();
                operationScope.Updated<Deal>(deal.Id)
                              .Complete();
            }
        }

        public void Add(Deal deal)
        {
            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(deal))
            {
                _identityProvider.SetFor(deal);
                _dealGenericSecureRepository.Add(deal);
                _dealGenericSecureRepository.Save();
                operationScope.Added<Deal>(deal.Id)
                              .Complete();
            }
        }

        public int Assign(Deal deal, long ownerCode)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Deal>())
            {
                var dealWithRelated = _finder.Find(Specs.Find.ById<Deal>(deal.Id))
                                             .Select(x => new
                                                 {
                                                     Orders = x.Orders.Where(y => !y.IsDeleted && y.WorkflowStepId != (int)OrderState.Archive && y.IsActive),
                                                     OrderPositions = x.Orders
                                                                       .Where(y => !y.IsDeleted && y.WorkflowStepId != (int)OrderState.Archive && y.IsActive)
                                                                       .SelectMany(y => y.OrderPositions).Where(op => !op.IsDeleted && op.IsActive),
                                                 })
                                             .Single();

                foreach (var order in dealWithRelated.Orders)
                {
                    order.OwnerCode = ownerCode;
                    _orderGenericRepository.Update(order);
                }

                _orderGenericRepository.Save();
                operationScope.Updated<Order>(dealWithRelated.Orders.Select(x => x.Id).ToArray())
                              .Complete();

                foreach (var orderPosition in dealWithRelated.OrderPositions)
                {
                    orderPosition.OwnerCode = ownerCode;
                    _orderPositionGenericRepository.Update(orderPosition);
                }

                _orderPositionGenericRepository.Save();

                operationScope.Updated<OrderPosition>(dealWithRelated.OrderPositions.Select(x => x.Id).ToArray())
                              .Complete();

                deal.OwnerCode = ownerCode;
                _dealGenericSecureRepository.Update(deal);

                var result = _dealGenericSecureRepository.Save();
                operationScope.Updated<Deal>(deal.Id)
                              .Complete();
                return result;
            }
        }

        public bool CheckIfDealHasOpenOrders(long dealId)
        {
           var releaseInfoQuery = _finder.FindAll<ReleaseInfo>();

           return _finder.Find<Order>(x => x.DealId == dealId && !x.IsDeleted && x.IsActive)
                .Any(o => o.WorkflowStepId != (int)OrderState.Rejected
                          && o.WorkflowStepId != (int)OrderState.Archive
                          && !(o.WorkflowStepId == (int)OrderState.OnTermination &&
                               releaseInfoQuery.Any(r => r.IsActive && !r.IsDeleted && !r.IsBeta &&
                                                         (r.Status == (int)ReleaseStatus.InProgressInternalProcessingStarted 
                                                         || r.Status == (int)ReleaseStatus.InProgressWaitingExternalProcessing 
                                                         || r.Status == (int)ReleaseStatus.Success)
                                                         && r.OrganizationUnit == o.DestOrganizationUnit &&
                                                         r.PeriodStartDate <= o.RejectionDate
                                                         && o.RejectionDate <= r.PeriodEndDate)));
        }

        public void CloseDeal(Deal deal, CloseDealReason closeReason, string closeReasonOther, string comment)
        {
            deal.CloseReason = (int)closeReason;
            deal.CloseReasonOther = closeReasonOther;
            deal.Comment = comment;
            deal.IsActive = false;

            // trim CloseDate to seconds - this is required by dynamics crm replication
            deal.CloseDate = DateTime.UtcNow.TrimToSeconds();

            Update(deal);
        }

        public ClientAndFirmForDealInfo GetClientAndFirmForDealInfo(Deal deal)
        {
            var clientInfo = _finder.Find(Specs.Find.ById<Client>(deal.ClientId) && Specs.Find.ActiveAndNotDeleted<Client>())
                                    .Select(x => new
                                        {
                                            ClientId = x.Id,
                                            MainFirm = x.Firms.FirstOrDefault(y => !y.IsDeleted && y.IsActive && y.Id == deal.MainFirmId),
                                        })
                                    .SingleOrDefault();

            return clientInfo != null
                       ? new ClientAndFirmForDealInfo
                           {
                               Client = _finder.FindOne(Specs.Find.ById<Client>(clientInfo.ClientId)),
                               MainFirm = clientInfo.MainFirm
                           }
                       : null;
        }

        public void ReopenDeal(Deal deal)
        {
            deal.CloseDate = null;
            deal.IsActive = true;
            deal.CloseReason = (int)CloseDealReason.None;
            deal.CloseReasonOther = null;
            deal.Comment = null;
            Update(deal);
        }

        public int SetOrderApprovedForReleaseStage(long dealId)
        {
            var deal = _finder.Find(Specs.Find.ById<Deal>(dealId)).Single();
            if (deal.DealStage == (int)DealStage.Service || deal.DealStage == (int)DealStage.OrderApprovedForRelease)
            {
                return 0;
            }

            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(deal))
            {
                deal.DealStage = (int)DealStage.OrderApprovedForRelease;
                _dealGenericSecureRepository.Update(deal);
                var result = _dealGenericSecureRepository.Save();
                operationScope.Updated<Deal>(deal.Id).Complete();
                return result;
            }
        }

        public int SetOrderFormedStage(long dealId, long orderId)
        {
            var orderStates = new[] { (int)OrderState.Approved, (int)OrderState.OnTermination, (int)OrderState.Archive };

            var otherOrdersExists = _finder.Find(OrderSpecs.Orders.Find.ForDeal(dealId)
                                                    && Specs.Find.ActiveAndNotDeleted<Order>())
                                           .Any(x => x.Id != orderId && orderStates.Contains(x.WorkflowStepId));

            if (otherOrdersExists)
            {
                return 0;
            }

            var deal = _finder.Find(Specs.Find.ById<Deal>(dealId)).Single();
            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(deal))
            {
                deal.DealStage = (int)DealStage.OrderFormed;
                _dealGenericSecureRepository.Update(deal);
                var result = _dealGenericSecureRepository.Save();
                operationScope.Updated<Deal>(deal.Id).Complete();
                return result;
            }
        }

        public int DecreaseDealEstimatedProfit(Deal deal, decimal estimatedProfitDelta)
        {
            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(deal))
            {
                deal.EstimatedProfit -= estimatedProfitDelta;
                _dealGenericSecureRepository.Update(deal);
                var result = _dealGenericSecureRepository.Save();
                operationScope.Updated<Deal>(deal.Id).Complete();
                return result;
            }
        }

        int IAssignAggregateRepository<Deal>.Assign(long entityId, long ownerCode)
        {
            var entity = _finder.Find(Specs.Find.ById<Deal>(entityId)).Single();
            return Assign(entity, ownerCode);
        }

        ChangeAggregateClientValidationResult IChangeAggregateClientRepository<Deal>.Validate(long entityId, long currentUserCode, long reserveCode)
        {
            var warnings = new List<string>();
            var securityErrors = new List<string>();
            var domainErrors = new List<string>();
            var result = new ChangeAggregateClientValidationResult(warnings, securityErrors, domainErrors);

            var dealInfo = _finder.Find(Specs.Find.ById<Deal>(entityId))
                .Select(x => new
                    {
                        x.Id,
                        x.IsActive,
                        x.OwnerCode
                    })
                .SingleOrDefault();
            if (dealInfo == null)
            {
                domainErrors.Add(BLResources.CouldNotFindDeal);
                return result;
            }

            if (!dealInfo.IsActive)
            {
                domainErrors.Add(BLResources.DealMustBeActive);
                return result;
            }

            // validate security
            if (!_entityAccessService.HasEntityAccess(EntityAccessTypes.Update, EntityName.Deal, currentUserCode, dealInfo.Id, dealInfo.OwnerCode, null))
            {
                securityErrors.Add(BLResources.YouHasNoEntityAccessPrivilege);
                return result;
            }

            return result;
        }

        int IChangeAggregateClientRepository<Deal>.ChangeClient(long entityId, long clientId, long currentUserCode, bool bypassValidation)
        {
            var deal = _finder.Find(Specs.Find.ById<Deal>(entityId)).Single();
            var firms = _finder.Find<Firm>(x => x.ClientId == clientId).ToArray();
            if (firms.Length == 1)
            {
                deal.MainFirmId = firms[0].Id;
            }
            else
            {
                deal.MainFirmId = null;
            }

            var client = _finder.Find<Client>(x => x.Id == clientId).SingleOrDefault();
            if (client == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound, "clientId");
            }

            // FIXME {all, 2013-09-24}: Приехало из 1.0. Используется операция UpdateIdentity, поскольку тут не происходит корректного Assign. 
            //                         Нужно решить, как вызвать оригинальную операцию, а не китайску подделку.
            //                         Пока оставляю так как есть, поскольку никто не жалуется и не факт, что после фикса не появятся проблемы.\
            // COMMENT {all, 27.09.2013}: Тут нужно скорее использовать конкретную операцию ChangeClient и декларировать в scope изменяемые сущности, 
            //    если при этом в коде вышестоящего по controlflow operationsservice не останется ничего кроме вызова разлиных aggregate service - пусть так, возможно, позднее решим избавится от необходимости создавать operationservice и aggregate service, если первый фактически ничего не делает
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                var orders = _finder.Find<Order>(x => x.DealId == entityId).ToArray();
                foreach (var order in orders)
                {
                    order.OwnerCode = client.OwnerCode;
                    _orderGenericRepository.Update(order);
                    operationScope.Updated<Order>(order.Id);
                }

                _orderGenericRepository.Save();
                operationScope.Complete();
            }

            deal.ClientId = clientId;
            deal.OwnerCode = client.OwnerCode;
            _dealGenericSecureRepository.Update(deal);
            return _dealGenericSecureRepository.Save();
        }

        public DealLegalPersonDto GetDealLegalPerson(long dealId)
        {
            var dealInfo = _finder.Find<Deal>(deal => deal.Id == dealId && !deal.IsDeleted)
                .Select(x => new DealLegalPersonDto
                    {
                        Id = x.Id, 
                        Name = x.Name, 
                        ClientId = x.ClientId, 
                        MainFirmId = x.MainFirmId, 
                        CurrencyId = x.CurrencyId,
                        OwnerCode = x.OwnerCode
                    })
                .SingleOrDefault();

            var legalPersonInfo = _finder.Find<LegalPerson>(lp => lp.IsActive && !lp.IsDeleted && lp.Client.IsActive && !lp.Client.IsDeleted && lp.ClientId == dealInfo.ClientId)
                .Select(x => new DealLegalPersonDto.LegalPersonDto { Id = x.Id, Name = x.LegalName })
                .Take(2)
                .ToArray();

            // Если не удаётся установить юрлицо однозначно - считаем, что значения по-умолчанию нет.
            if (dealInfo != null && legalPersonInfo.Length == 1)
            {
                dealInfo.LegalPerson = legalPersonInfo.Single();
            }

            return dealInfo;
        }
    }
}