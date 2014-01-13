using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class ClientOperationsExportService : OperationsExportService<Client, ExportFlowFinancialDataClient>,
                                                 IGenericOperationsExportService<Client, ExportFlowFinancialDataClient>
    {
        public ClientOperationsExportService(IExportableOperationsPersistenceService<Client, ExportFlowFinancialDataClient> persistenceService,
                                             IOperationsExporter<Client, ExportFlowFinancialDataClient> operationsExporter) 
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowFinancialDataClient, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowFinancialDataClient, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowFinancialDataClient processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowFinancialDataClient CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowFinancialDataClient { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}