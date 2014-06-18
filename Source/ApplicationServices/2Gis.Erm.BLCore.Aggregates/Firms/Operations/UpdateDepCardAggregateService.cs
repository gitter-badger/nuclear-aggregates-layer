using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class UpdateDepCardAggregateService : IAggregatePartRepository<Firm>, IUpdateAggregateRepository<DepCard>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<DepCard> _depCardRepository;

        public UpdateDepCardAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<DepCard> depCardRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _depCardRepository = depCardRepository;
        }

        public int Update(DepCard entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DepCard>())
            {
                _depCardRepository.Update(entity);
                operationScope.Updated<DepCard>(entity.Id);

                var count = _depCardRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}