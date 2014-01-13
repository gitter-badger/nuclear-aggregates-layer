using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class ThemeOperationsExportService : OperationsExportService<Theme, ExportFlowOrdersTheme>,
                                                IGenericOperationsExportService<Theme, ExportFlowOrdersTheme>
    {
        public ThemeOperationsExportService(IExportableOperationsPersistenceService<Theme, ExportFlowOrdersTheme> persistenceService,
                                            IOperationsExporter<Theme, ExportFlowOrdersTheme> operationsExporter) 
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowOrdersTheme, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowOrdersTheme, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowOrdersTheme processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowOrdersTheme CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowOrdersTheme { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}