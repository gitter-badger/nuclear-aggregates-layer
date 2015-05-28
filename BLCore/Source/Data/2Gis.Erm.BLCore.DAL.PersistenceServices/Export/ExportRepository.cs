﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public sealed class ExportRepository<TEntity> : IExportRepository<TEntity> where TEntity : class, IEntity, IEntityKey
    {
        private readonly IQuery _query;
        private readonly IOldOperationContextParser _oldOperationContextParser;
        private readonly QueryRuleContainer<TEntity> _metadata;

        public ExportRepository(IQuery query, IExportMetadataProvider metadataProvider, IOldOperationContextParser oldOperationContextParser)
        {
            _query = query;
            _oldOperationContextParser = oldOperationContextParser;
            _metadata = metadataProvider.GetMetadata<TEntity>();
        }

        public IQueryBuilder<TEntity> GetBuilderForFailedObjects(IEnumerable<ExportFailedEntity> failedEntities)
        {
            return new FailedQueueQueryBuilder<TEntity>(_query, failedEntities);
        }

        public IQueryBuilder<TEntity> GetBuilderForOperations(IEnumerable<PerformedBusinessOperation> operations)
        {
            return new OperationsQueryBuilder<TEntity>(_query, operations, _metadata, _oldOperationContextParser);
        }

        public IEnumerable<TDto> GetEntityDtos<TDto>(IQueryBuilder<TEntity> queryBuilder,
                                                     SelectSpecification<TEntity, TDto> selectSpecification,
                                                     params FindSpecification<TEntity>[] filterSpecifications)
        {
            var query = queryBuilder.Create(filterSpecifications);
            return query.Select(selectSpecification).ToArray();
        }
    }
}
