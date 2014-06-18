using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class CreateDepCardAggregateService : IAggregatePartRepository<Firm>, ICreateAggregateRepository<DepCard>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<DepCard> _depCardRepository;

        public CreateDepCardAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<DepCard> depCardRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _depCardRepository = depCardRepository;
        }

        public int Create(DepCard entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DepCard>())
            {
                _depCardRepository.Add(entity);
                operationScope.Added<DepCard>(entity.Id);

                var count = _depCardRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}