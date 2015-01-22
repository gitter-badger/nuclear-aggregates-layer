using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkCreateCardRelationsAggregateService : IBulkCreateCardRelationsAggregateService
    {
        private readonly IRepository<CardRelation> _cardRelationRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkCreateCardRelationsAggregateService(IRepository<CardRelation> cardRelationRepository, IOperationScopeFactory scopeFactory)
        {
            _cardRelationRepository = cardRelationRepository;
            _scopeFactory = scopeFactory;
        }

        public void Create(IReadOnlyCollection<CardRelation> cardRelationsToCreate)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkCreateIdentity, CardRelation>())
            {
                _cardRelationRepository.AddRange(cardRelationsToCreate);
                _cardRelationRepository.Save();

                scope.Added(cardRelationsToCreate.AsEnumerable()).Complete();
            }
        }
    }
}