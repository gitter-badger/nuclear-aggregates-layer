using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class ThemeOrganizationUnitOperationsExportService : OperationsExportService<ThemeOrganizationUnit, ExportFlowOrdersThemeBranch>,
                                                                IGenericOperationsExportService<ThemeOrganizationUnit, ExportFlowOrdersThemeBranch>
    {
        public ThemeOrganizationUnitOperationsExportService(
            IExportableOperationsPersistenceService<ThemeOrganizationUnit, ExportFlowOrdersThemeBranch> persistenceService,
            IOperationsExporter<ThemeOrganizationUnit, ExportFlowOrdersThemeBranch> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowOrdersThemeBranch, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowOrdersThemeBranch, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowOrdersThemeBranch processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowOrdersThemeBranch CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowOrdersThemeBranch { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}