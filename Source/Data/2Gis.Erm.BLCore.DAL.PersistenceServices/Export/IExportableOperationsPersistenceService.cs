using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public interface IExportableOperationsPersistenceService : ISimplifiedPersistenceService
    {
        IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate, int maxOperationCount);
        IEnumerable<ExportFailedEntity> GetFailedEntities(int maxEntitiesCount, int skipCount);
        void InsertToFailureQueue(IEnumerable<IExportableEntityDto> failedObjects);
        void RemoveFromFailureQueue(IEnumerable<IExportableEntityDto> exportedObjects);
    }

    public interface IExportableOperationsPersistenceService<TEntity, TProcessedOperationEntity> : IExportableOperationsPersistenceService
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey
    {
        int SaveProcessedOperations(IEnumerable<PerformedBusinessOperation> operations,
                                   Func<PerformedBusinessOperation, TProcessedOperationEntity> processedOperationEntityCreator,
                                   Action<TProcessedOperationEntity> processedOperationEntityUpdater);
        DateTime GetLastProcessedOperationPerformDate(ISelectSpecification<TProcessedOperationEntity, DateTime> selectSortFieldSpecification);
    }
}
