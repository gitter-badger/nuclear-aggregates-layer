using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkUpdateDepCardAggregateService : IBulkUpdateDepCardAggregateService
    {
        private readonly IRepository<DepCard> _depCardRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkUpdateDepCardAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<DepCard> depCardRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _depCardRepository = depCardRepository;
        }

        public void Update(IReadOnlyCollection<DepCard> depCards)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DepCard>())
            {
                foreach (var depCard in depCards)
                {
                    _depCardRepository.Update(depCard);
                    scope.Updated(depCard);
                }

                _depCardRepository.Save();
                scope.Complete();
            }
        }
    }
}