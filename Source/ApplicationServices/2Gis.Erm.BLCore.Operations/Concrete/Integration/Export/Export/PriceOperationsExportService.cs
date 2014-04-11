using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class PriceOperationsExportService : OperationsExportService<Price, ExportFlowPriceListsPriceList>,
                                                IGenericOperationsExportService<Price, ExportFlowPriceListsPriceList>
    {
        public PriceOperationsExportService(IExportableOperationsPersistenceService<Price, ExportFlowPriceListsPriceList> persistenceService,
                                            IOperationsExporter<Price, ExportFlowPriceListsPriceList> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowPriceListsPriceList, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowPriceListsPriceList, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowPriceListsPriceList processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowPriceListsPriceList CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowPriceListsPriceList { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}