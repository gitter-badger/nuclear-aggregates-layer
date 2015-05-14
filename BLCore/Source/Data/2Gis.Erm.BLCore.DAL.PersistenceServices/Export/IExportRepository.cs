using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public interface IExportRepository : ISimplifiedModelConsumer
    {
    }

    public interface IExportRepository<T> : IExportRepository where T : class, IEntity, IEntityKey
    {
        IQueryBuilder<T> GetBuilderForOperations(IEnumerable<PerformedBusinessOperation> operations);
        IQueryBuilder<T> GetBuilderForFailedObjects(IEnumerable<ExportFailedEntity> failedEntities);
        IEnumerable<TDto> GetEntityDtos<TDto>(IQueryBuilder<T> queryBuilder,
                                              SelectSpecification<T, TDto> selectSpecification,
                                              params FindSpecification<T>[] filterSpecifications);
    }
}
