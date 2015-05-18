using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public sealed class ExportRepository<TEntity> : IExportRepository<TEntity> where TEntity : class, IEntity, IEntityKey
    {
        private readonly IFinder _finder;
        private readonly IOldOperationContextParser _oldOperationContextParser;
        private readonly QueryRuleContainer<TEntity> _metadata;

        public ExportRepository(IFinder finder, IExportMetadataProvider metadataProvider, IOldOperationContextParser oldOperationContextParser)
        {
            _finder = finder;
            _oldOperationContextParser = oldOperationContextParser;
            _metadata = metadataProvider.GetMetadata<TEntity>();
        }

        public IQueryBuilder<TEntity> GetBuilderForFailedObjects(IEnumerable<ExportFailedEntity> failedEntities)
        {
            return new FailedQueueQueryBuilder<TEntity>(_finder, failedEntities);
        }

        public IQueryBuilder<TEntity> GetBuilderForOperations(IEnumerable<PerformedBusinessOperation> operations)
        {
            return new OperationsQueryBuilder<TEntity>(_finder, operations, _metadata, _oldOperationContextParser);
        }

        public IEnumerable<TDto> GetEntityDtos<TDto>(IQueryBuilder<TEntity> queryBuilder,
                                                     ISelectSpecification<TEntity, TDto> selectSpecification,
                                                     params IFindSpecification<TEntity>[] filterSpecifications)
        {
            var query = queryBuilder.Create(filterSpecifications);
            return query.Select(selectSpecification.Selector).ToArray();
        }
    }
}
