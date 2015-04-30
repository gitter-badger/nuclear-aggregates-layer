using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkUpdateCardRelationsAggregateService : IBulkUpdateCardRelationsAggregateService
    {
        private readonly IRepository<CardRelation> _cardRelationRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkUpdateCardRelationsAggregateService(IRepository<CardRelation> cardRelationRepository, IOperationScopeFactory scopeFactory)
        {
            _cardRelationRepository = cardRelationRepository;
            _scopeFactory = scopeFactory;
        }

        public void Update(IReadOnlyCollection<CardRelation> cardRelationsToUpdate)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkUpdateIdentity, CardRelation>())
            {
                foreach (var cardRelation in cardRelationsToUpdate)
                {
                    _cardRelationRepository.Update(cardRelation);
                }

                _cardRelationRepository.Save();
                scope.Updated(cardRelationsToUpdate.AsEnumerable()).Complete();
            }
        }
    }
}