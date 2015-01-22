using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    /// <summary>
    /// Контракт для особого класс функциональности, который обеспечивает непосредственно специфическую реакцию на факт выполнения в системе каких-то коннкретных businessoperations
    /// Пока основной тип реакции - выгрузка затронутых операцией сущностей в соответствующие потоки корпоративной шины интеграции.
    /// Наиболее близкий аналог - operationservice, однако, operationservice, который обрабатывает последствия выполнения ругих operationservices выглядит немного более сложно, чем это необходимо
    /// </summary>
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
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IIntegrationProcessorState
    {
    }
}