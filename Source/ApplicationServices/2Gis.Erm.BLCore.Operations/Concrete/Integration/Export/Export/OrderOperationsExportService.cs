using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public class OrderOperationsExportService : OperationsExportService<Order, ExportFlowOrdersOrder>,
                                                IGenericOperationsExportService<Order, ExportFlowOrdersOrder>
    {
        public OrderOperationsExportService(IExportableOperationsPersistenceService<Order, ExportFlowOrdersOrder> persistenceService,
                                            IOperationsExporter<Order, ExportFlowOrdersOrder> operationsExporter) 
            : base(persistenceService, operationsExporter)
        {
        }

        protected override ISelectSpecification<ExportFlowOrdersOrder, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<ExportFlowOrdersOrder, DateTime>(operation => operation.Date); }
        }

        protected override void UpdateProcessedOperation(ExportFlowOrdersOrder processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected override ExportFlowOrdersOrder CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new ExportFlowOrdersOrder { Id = operation.Id, Date = DateTime.UtcNow };
        }
    }
}