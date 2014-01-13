using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class FirmAddressOperationsExportService : OperationsExportService<FirmAddress, ExportFlowCardExtensionsCardCommercial>,
                                                      IGenericOperationsExportService<FirmAddress, ExportFlowCardExtensionsCardCommercial>
    {
        public FirmAddressOperationsExportService(
            IExportableOperationsPersistenceService<FirmAddress, ExportFlowCardExtensionsCardCommercial> persistenceService,
            IOperationsExporter<FirmAddress, ExportFlowCardExtensionsCardCommercial> operationsExporter) 
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowCardExtensionsCardCommercial, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowCardExtensionsCardCommercial, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowCardExtensionsCardCommercial processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowCardExtensionsCardCommercial CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowCardExtensionsCardCommercial { Id = operation.Id, Date = DateTime.UtcNow }; 
        }
    }
}