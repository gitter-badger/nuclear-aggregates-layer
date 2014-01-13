using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class ImportedFirmAddressOperationsExportService : OperationsExportService<FirmAddress, ImportedFirmAddress>,
                                                              IGenericOperationsExportService<FirmAddress, ImportedFirmAddress>
    {
        public ImportedFirmAddressOperationsExportService(IExportableOperationsPersistenceService<FirmAddress, ImportedFirmAddress> persistenceService,
                                                          IOperationsExporter<FirmAddress, ImportedFirmAddress> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ImportedFirmAddress, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ImportedFirmAddress, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ImportedFirmAddress processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ImportedFirmAddress CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ImportedFirmAddress { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}