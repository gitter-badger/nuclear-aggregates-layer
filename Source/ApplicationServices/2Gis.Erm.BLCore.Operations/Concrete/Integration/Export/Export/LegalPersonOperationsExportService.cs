using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class LegalPersonOperationsExportService : OperationsExportService<LegalPerson, ExportFlowFinancialDataLegalEntity>,
                                                       IGenericOperationsExportService<LegalPerson, ExportFlowFinancialDataLegalEntity>
    {
        public LegalPersonOperationsExportService(IExportableOperationsPersistenceService<LegalPerson, ExportFlowFinancialDataLegalEntity> persistenceService,
                                                  IOperationsExporter<LegalPerson, ExportFlowFinancialDataLegalEntity> operationsExporter) 
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowFinancialDataLegalEntity, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowFinancialDataLegalEntity, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowFinancialDataLegalEntity processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowFinancialDataLegalEntity CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowFinancialDataLegalEntity { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}