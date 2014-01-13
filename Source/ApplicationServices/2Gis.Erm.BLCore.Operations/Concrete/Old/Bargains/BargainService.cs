using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bargains
{
    // FIXME {all, 21.10.2013}: фактически набор OperationServices зачем-то слепленных в один интерфейс
    public sealed class BargainService : IBargainService
    {
        private static readonly ISelectSpecification<Order, OrderBatchClass> BatchSelector =
            new SelectSpecification<Order, OrderBatchClass>(x => new OrderBatchClass
                {
                    Order = x,
                    LegalPersonId = x.LegalPersonId,
                    BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                    OrderSignupDate = x.SignupDate,
                    BargainTypeId = x.BranchOfficeOrganizationUnit.BranchOffice.BargainTypeId,
                    DestinationSyncCode1C = x.DestOrganizationUnit.SyncCode1C,
                    SourceSyncCode1C = x.SourceOrganizationUnit.SyncCode1C,
                });

        private readonly IBargainRepository _bargainRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IOperationScopeFactory _scopeFactory;

        // FIXME {all, 06.08.2013}: почему то в слое operation services используется finder 
        public BargainService(
            IBargainRepository bargainRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IFinder finder,
            IOperationScopeFactory scopeFactory)
        {
            _bargainRepository = bargainRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _finder = finder;
            _scopeFactory = scopeFactory;
        }

        public long GenerateNextBargainUniqueNumber()
        {
            return _bargainRepository.GenerateNextBargainUniqueNumber();
        }

        public Bargain GetById(long id)
        {
            return _bargainRepository.Find(id);
        }

        public GetOrderBargainResult GetBargain(long? branchOfficeOrganizationUnitId, long? legalPersonId, DateTime orderSignupDate)
        {
            var result = new GetOrderBargainResult();
            if (branchOfficeOrganizationUnitId == 0 || legalPersonId == 0)
            {
                return result;
            }

            var specification = OrderSpecs.Bargains.Find.ForOrder(legalPersonId, branchOfficeOrganizationUnitId);

            var bargainsArray = _bargainRepository.FindBySpecification(specification).ToArray();

            if (bargainsArray.Length == 0)
            {
                return result;
            }

            Bargain currentBargain;
            if (bargainsArray.Length == 1)
            {
                currentBargain = bargainsArray[0];
            }
            else
            {
                currentBargain = bargainsArray.FirstOrDefault(x => x.ClosedOn == null || x.ClosedOn >= orderSignupDate)
                                 ?? bargainsArray.First();
            }

            result.BargainId = currentBargain.Id;
            result.BargainNumber = currentBargain.Number;
            result.BargainClosedOn = currentBargain.ClosedOn;

            return result;
        }

        public EntityIdAndNumber CreateBargainForOrder(long orderId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var batch = _finder.Find<Order, OrderBatchClass>(BatchSelector, Specs.Find.ById<Order>(orderId))
                                   .Single();

                if (!batch.LegalPersonId.HasValue)
                {
                    throw new NotificationException(BLResources.LegalPersonNotFound);
                }

                if (!batch.BranchOfficeOrganizationUnitId.HasValue)
                {
                    throw new NotificationException(BLResources.BranchOfficeOrganizationUnitNotFound);
                }

                var existingBargain = _bargainRepository.FindBySpecification(OrderSpecs.Bargains.Find.Actual(batch.LegalPersonId,
                                                                                                            batch.BranchOfficeOrganizationUnitId,
                                                                                                            batch.OrderSignupDate))
                                                        .FirstOrDefault();

                if (existingBargain != null)
                {
                    // Если для данных юр. лица клиента и юр. лица исполнителя существует договор,
                    // незакрытый на момент подписания заказа, то возвращаем его.
                    return new EntityIdAndNumber(existingBargain.Id, existingBargain.Number);
                }

                var bargain = new Bargain
                    {
                        CustomerLegalPersonId = (long)batch.LegalPersonId,
                        ExecutorBranchOfficeId = batch.BranchOfficeOrganizationUnitId.Value,
                        BargainTypeId = batch.BargainTypeId.Value,
                        SignedOn = DateTime.UtcNow.Date,
                        OwnerCode = _userContext.Identity.Code,
                        HasDocumentsDebt = (byte)DocumentsDebt.Absent,
                        IsActive = true,
                    };

                using (var operationScope = _scopeFactory.CreateNonCoupled<BindToOrderIdentity>())
                {
                    AddBargain(batch, bargain, GenerateNextBargainUniqueNumber());

                    operationScope.Added<Bargain>(bargain.Id)
                                  .Updated<Order>(batch.Order.Id)
                                  .Complete();
                }

                transaction.Complete();
                return new EntityIdAndNumber(bargain.Id, bargain.Number);
            }
        }

        public CloseBargainsResult CloseBargains(DateTime closeDate)
        {
            List<Bargain> bargains = _bargainRepository.FindBySpecification(OrderSpecs.Bargains.Find.NonClosed).ToList();

            var invalidPeriodBargainsNumbers = bargains.Where(x => x.SignedOn > closeDate).Select(x => x.Number).ToArray();

            var response = new CloseBargainsResult();
            if (invalidPeriodBargainsNumbers.Length > 0)
            {
                response.NonClosedBargainsNumbers = invalidPeriodBargainsNumbers;
            }

            _bargainRepository.CloseBargains(bargains.Where(x => x.SignedOn <= closeDate), closeDate);

            return response;
        }

        private void AddBargain(OrderBatchClass batch, Bargain bargain, long? reservedNumberDigit)
        {
            var order = batch.Order;

            if (reservedNumberDigit == null)
            {
                throw new InvalidOperationException(EnumResources.FailedToGetNumberForBargain);
            }

            bargain.Number = string.Format(BLResources.BargainNumberTemplate, batch.SourceSyncCode1C, batch.DestinationSyncCode1C, reservedNumberDigit.Value);

            using (var scope = _unitOfWork.CreateScope())
            {
                var bargainRepository = scope.CreateRepository<IBargainRepository>();
                bargainRepository.CreateOrUpdate(bargain);
                scope.Complete();
            }

            order.BargainId = bargain.Id;
            _orderRepository.Update(order);
        }

        private sealed class OrderBatchClass
        {
            public long? LegalPersonId { get; set; }
            public long? BranchOfficeOrganizationUnitId { get; set; }
            public long? BargainTypeId { get; set; }
            public DateTime OrderSignupDate { get; set; }
            public Order Order { get; set; }
            public string DestinationSyncCode1C { get; set; }
            public string SourceSyncCode1C { get; set; }
        }
    }
}
