using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bargains
{
    public class UpdateBargainAggregateService : IAggregateRootRepository<Order>, IUpdateAggregateRepository<Bargain>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Bargain> _entitySecureRepository;

        public UpdateBargainAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Bargain> entitySecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _entitySecureRepository = entitySecureRepository;
        }

        public int Update(Bargain entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Bargain>())
            {
                _entitySecureRepository.Update(entity);
                operationScope.Updated<Bargain>(entity.Id);

                var count = _entitySecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}