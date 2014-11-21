using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Processors.Concrete
{
    public sealed class PositionOperationsExportService : IntegrationProcessorOperationService<Position, ExportFlowNomenclatures_NomenclatureElement>
    {
        public PositionOperationsExportService(
            IOperationsProcessingsStoreService<Position, ExportFlowNomenclatures_NomenclatureElement> processingsStoreService,
            IOperationsExporter<Position, ExportFlowNomenclatures_NomenclatureElement> operationsExporter)
            : base(processingsStoreService, operationsExporter)
        {
        }
    }
}