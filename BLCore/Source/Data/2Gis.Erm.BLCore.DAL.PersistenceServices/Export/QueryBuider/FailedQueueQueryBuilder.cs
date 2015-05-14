using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider
{
    public sealed class FailedQueueQueryBuilder<TEntity> : IQueryBuilder<TEntity>
        where TEntity : class, IEntityKey, IEntity
    {
        private readonly IFinder _finder;
        private readonly IEnumerable<ExportFailedEntity> _failedEntities;

        public FailedQueueQueryBuilder(IFinder finder, IEnumerable<ExportFailedEntity> failedEntities)
        {
            _finder = finder;
            _failedEntities = failedEntities;
        }

        public IQueryable<TEntity> Create(params FindSpecification<TEntity>[] filterSpecifications)
        {
            var ids = _failedEntities
                            .Select(entity => entity.EntityId)
                            .Distinct()
                            .ToArray();

            var findByIdsQuery = _finder.Find(Specs.Find.ByIds<TEntity>(ids));
            return filterSpecifications.Aggregate(findByIdsQuery, (current, filter) => current.Where(filter));
        }
    }
}
