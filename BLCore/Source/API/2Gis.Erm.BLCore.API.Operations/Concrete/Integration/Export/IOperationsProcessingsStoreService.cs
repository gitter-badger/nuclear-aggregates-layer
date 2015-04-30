using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public interface IOperationsProcessingsStoreService : ISimplifiedModelConsumer
    {
        IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate);
        IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate, int maxOperationCount);
        IEnumerable<ExportFailedEntity> GetFailedEntities();
        IEnumerable<ExportFailedEntity> GetFailedEntities(int maxEntitiesCount, int skipCount);
        void InsertToFailureQueue(IEnumerable<IExportableEntityDto> failedObjects);
        void RemoveFromFailureQueue(IEnumerable<IExportableEntityDto> exportedObjects);
    }

    public interface IOperationsProcessingsStoreService<TEntity, TProcessedOperationEntity> : IOperationsProcessingsStoreService
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey
    {
        int SaveProcessedOperations(IEnumerable<PerformedBusinessOperation> operations,
                                   Func<PerformedBusinessOperation, TProcessedOperationEntity> processedOperationEntityCreator,
                                   Action<TProcessedOperationEntity> processedOperationEntityUpdater);
        DateTime GetLastProcessedOperationPerformDate(ISelectSpecification<TProcessedOperationEntity, DateTime> selectSortFieldSpecification);
    }
}
