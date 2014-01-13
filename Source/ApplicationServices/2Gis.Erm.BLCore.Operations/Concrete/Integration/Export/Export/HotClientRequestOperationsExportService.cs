using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public sealed class HotClientRequestOperationsExportService : OperationsExportService<HotClientRequest, ExportToMsCrm_HotClients>,
                                                                  IGenericOperationsExportService<HotClientRequest, ExportToMsCrm_HotClients>
    {
        public HotClientRequestOperationsExportService(
            IExportableOperationsPersistenceService<HotClientRequest, ExportToMsCrm_HotClients> persistenceService,
            IOperationsExporter<HotClientRequest, ExportToMsCrm_HotClients> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportToMsCrm_HotClients, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportToMsCrm_HotClients, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportToMsCrm_HotClients processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportToMsCrm_HotClients CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportToMsCrm_HotClients { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}
