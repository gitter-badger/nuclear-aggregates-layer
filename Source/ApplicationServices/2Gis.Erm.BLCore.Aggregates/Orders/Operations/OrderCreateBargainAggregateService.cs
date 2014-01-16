using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderCreateBargainAggregateService : IOrderCreateBargainAggregateService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Bargain> _bargainRepository;
        private readonly IEvaluateBargainNumberService _evaluateBargainNumberService;
        private readonly IBargainPersistenceService _bargainPersistenceService;
        private readonly IIdentityProvider _identityProvider;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderCreateBargainAggregateService(
            IRepository<Order> orderRepository,
            IRepository<Bargain> bargainRepository,
            IEvaluateBargainNumberService evaluateBargainNumberService,
            IBargainPersistenceService bargainPersistenceService, 
            IIdentityProvider identityProvider,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory)
        {
            _orderRepository = orderRepository;
            _bargainRepository = bargainRepository;
            _evaluateBargainNumberService = evaluateBargainNumberService;
            _bargainPersistenceService = bargainPersistenceService;
            _identityProvider = identityProvider;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
        }

        public Bargain Create(CreateBargainInfo createBargainInfo)
        {
            var bargain = new Bargain
                {
                    CustomerLegalPersonId = createBargainInfo.LegalPersonId.Value,
                    ExecutorBranchOfficeId = createBargainInfo.BranchOfficeOrganizationUnitId.Value,
                    BargainTypeId = createBargainInfo.BargainTypeId.Value,
                    SignedOn = DateTime.UtcNow.Date,
                    OwnerCode = _userContext.Identity.Code,
                    HasDocumentsDebt = (byte)DocumentsDebt.Absent,
                    IsActive = true,
                };

            // FIXME {all, 14.01.2014}: при рефакторинге BargainService и выделение CreateBargainForOrder в самостоятельный operation service нужно избавиться/зарефакторитьBindToOrderIdentity 
            // - логировать нужно createidentity договора, BindToOrderIdentity либо вообще удалить, переименовать в AppendBargain, AttachBargain, BindBargain и т.п. (заиспользовав тот же id операции)
            using (var scope = _scopeFactory.CreateNonCoupled<BindToOrderIdentity>())
            {
                var bargainUniqueIndex = _bargainPersistenceService.GenerateNextBargainUniqueNumber();
                bargain.Number = _evaluateBargainNumberService.Evaluate(createBargainInfo.SourceSyncCode1C, createBargainInfo.DestinationSyncCode1C, bargainUniqueIndex);
                
                _identityProvider.SetFor(bargain);
                _bargainRepository.Add(bargain);

                createBargainInfo.Order.BargainId = bargain.Id;
                _orderRepository.Update(createBargainInfo.Order);

                _bargainRepository.Save();
                _orderRepository.Save();

                scope.Added<Bargain>(bargain.Id)
                     .Updated<Order>(createBargainInfo.Order.Id)
                     .Complete();
            }

            return bargain;
        }
    }
}