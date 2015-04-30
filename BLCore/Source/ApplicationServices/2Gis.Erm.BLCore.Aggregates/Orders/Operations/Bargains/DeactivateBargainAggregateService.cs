using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bargains
{
    public class DeactivateBargainAggregateService : IAggregateRootRepository<Order>, IDeactivateAggregateRepository<Bargain>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Bargain> _entityRepository;
        private readonly IOrderReadModel _orderReadModel;

        public DeactivateBargainAggregateService(IOperationScopeFactory operationScopeFactory,
                                                 ISecureRepository<Bargain> entityRepository,
                                                 IOrderReadModel orderReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _entityRepository = entityRepository;
            _orderReadModel = orderReadModel;
        }

        public int Deactivate(Bargain bargain)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, Bargain>())
            {
                bargain.IsActive = false;
                _entityRepository.Update(bargain);
                operationScope.Updated<Bargain>(bargain.Id);

                var count = _entityRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Deactivate(long entityId)
        {
            return Deactivate(_orderReadModel.GetBargain(entityId));
        }
    }
}