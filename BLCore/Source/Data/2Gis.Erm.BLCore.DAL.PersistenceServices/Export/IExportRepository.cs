using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Simplified;

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
                                              ISelectSpecification<T, TDto> selectSpecification,
                                              params IFindSpecification<T>[] filterSpecifications);
    }
}
