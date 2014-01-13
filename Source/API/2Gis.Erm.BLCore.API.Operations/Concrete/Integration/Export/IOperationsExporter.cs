using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    // TODO {d.ivanov, 28.08.2013}: Возможно, IOperationsExporter - это persistence сервис?
    public interface IOperationsExporter : ISimplifiedModelConsumer
    {
        void ExportOperations(FlowDescription flowDescription,
                              IEnumerable<PerformedBusinessOperation> operations,
                              int packageSize,
                              out IEnumerable<IExportableEntityDto> failedEntites);

        void ExportFailedEntities(FlowDescription flowDescription,
                                  IEnumerable<ExportFailedEntity> failedEntities,
                                  int packageSize,
                                  out IEnumerable<IExportableEntityDto> exportedEntites);
    }

    public interface IOperationsExporter<TEntity, TProcessedOperationEntity> : IOperationsExporter 
        where TEntity : class, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey
    {
    }
}