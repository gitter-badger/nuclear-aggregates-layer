using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class AdvertismentOperationsExportService : OperationsExportService<Advertisement, ExportFlowOrdersAdvMaterial>,
                                                       IGenericOperationsExportService<Advertisement, ExportFlowOrdersAdvMaterial>
    {
        public AdvertismentOperationsExportService(IExportableOperationsPersistenceService<Advertisement, ExportFlowOrdersAdvMaterial> persistenceService,
                                                   IOperationsExporter<Advertisement, ExportFlowOrdersAdvMaterial> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowOrdersAdvMaterial, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowOrdersAdvMaterial, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowOrdersAdvMaterial processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowOrdersAdvMaterial CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowOrdersAdvMaterial { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}