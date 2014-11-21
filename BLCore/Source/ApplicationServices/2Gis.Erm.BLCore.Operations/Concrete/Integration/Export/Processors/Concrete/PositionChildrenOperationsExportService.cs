using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Processors.Concrete
{
    public sealed class PositionChildrenOperationsExportService :
        IntegrationProcessorOperationService<PositionChildren, ExportFlowNomenclatures_NomenclatureElementRelation>
    {
        public PositionChildrenOperationsExportService(
            IOperationsProcessingsStoreService<PositionChildren, ExportFlowNomenclatures_NomenclatureElementRelation> processingsStoreService,
            IOperationsExporter<PositionChildren, ExportFlowNomenclatures_NomenclatureElementRelation> operationsExporter)
            : base(processingsStoreService, operationsExporter)
        {
        }
    }
}