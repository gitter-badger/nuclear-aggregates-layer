using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bargains
{
    // FIXME {all, 21.10.2013}: фактически набор OperationServices зачем-то слепленных в один интерфейс - распилить, SRP и т.п.
    public sealed class BargainService : IBargainService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IBargainRepository _bargainRepository;
        private readonly IOrderCreateBargainAggregateService _orderCreateBargainAggregateService;

        public BargainService(
            IOrderReadModel orderReadModel,
            IBargainRepository bargainRepository,
            IOrderCreateBargainAggregateService orderCreateBargainAggregateService)
        {
            _orderReadModel = orderReadModel;
            _bargainRepository = bargainRepository;
            _orderCreateBargainAggregateService = orderCreateBargainAggregateService;
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
                var createBargainInfo = _orderReadModel.GetBargainInfoForCreate(orderId);
                if (!createBargainInfo.LegalPersonId.HasValue)
                {
                    throw new NotificationException(BLResources.LegalPersonNotFound);
                }

                if (!createBargainInfo.BranchOfficeOrganizationUnitId.HasValue)
                {
                    throw new NotificationException(BLResources.BranchOfficeOrganizationUnitNotFound);
                }

                Bargain existingBargain;
                if (_orderReadModel.TryGetExistingBargain(
                                        createBargainInfo.LegalPersonId.Value, 
                                        createBargainInfo.BranchOfficeOrganizationUnitId.Value, 
                                        createBargainInfo.OrderSignupDate, 
                                        out existingBargain))
                {
                    // Если для данных юр. лица клиента и юр. лица исполнителя существует договор,
                    // незакрытый на момент подписания заказа, то возвращаем его.
                    return new EntityIdAndNumber(existingBargain.Id, existingBargain.Number);
                }

                // FIXME {all, 14.01.2014}: при рефакторинге BargainService и выделение CreateBargainForOrder в самостоятельный operation service нужно избавиться/зарефакторить BindToOrderIdentity (см. aggregate сервис)
                // - логировать нужно createidentity договора, BindToOrderIdentity либо вообще удалить, переименовать в AppendBargain, AttachBargain, BindBargain и т.п. (заиспользовав тот же id операции)
                var createdBargain = _orderCreateBargainAggregateService.Create(createBargainInfo);
                
                transaction.Complete();
                return new EntityIdAndNumber(createdBargain.Id, createdBargain.Number);
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
    }
}
