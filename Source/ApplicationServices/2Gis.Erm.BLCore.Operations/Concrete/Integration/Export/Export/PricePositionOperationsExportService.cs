using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class PricePositionOperationsExportService : OperationsExportService<PricePosition, ExportFlowPriceListsPriceListPosition>,
                                                        IGenericOperationsExportService<PricePosition, ExportFlowPriceListsPriceListPosition>
    {
        public PricePositionOperationsExportService(IExportableOperationsPersistenceService<PricePosition, ExportFlowPriceListsPriceListPosition> persistenceService,
                                                    IOperationsExporter<PricePosition, ExportFlowPriceListsPriceListPosition> operationsExporter)
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowPriceListsPriceListPosition, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowPriceListsPriceListPosition, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowPriceListsPriceListPosition processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowPriceListsPriceListPosition CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowPriceListsPriceListPosition { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}