using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class ThemeTemplateOperationsExportService : OperationsExportService<ThemeTemplate, ExportFlowOrdersResource>,
                                                        IGenericOperationsExportService<ThemeTemplate, ExportFlowOrdersResource>
    {
        public ThemeTemplateOperationsExportService(IExportableOperationsPersistenceService<ThemeTemplate, ExportFlowOrdersResource> persistenceService,
                                                    IOperationsExporter<ThemeTemplate, ExportFlowOrdersResource> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowOrdersResource, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowOrdersResource, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowOrdersResource processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowOrdersResource CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowOrdersResource { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}