using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkCreateDepCardAggregateService : IBulkCreateDepCardAggregateService
    {
        private readonly IRepository<DepCard> _depCardRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateDepCardAggregateService(IRepository<DepCard> depCardRepository, IOperationScopeFactory operationScopeFactory)
        {
            _depCardRepository = depCardRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(IReadOnlyCollection<DepCard> depCards)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkCreateIdentity, DepCard>())
            {
                _depCardRepository.AddRange(depCards);
                _depCardRepository.Save();

                scope.Added(depCards.AsEnumerable())
                     .Complete();
            }
        }
    }
}