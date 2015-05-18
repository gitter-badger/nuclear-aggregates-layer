using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Integration.Export
{
    public sealed class PerfomedOperationsProcessorOperationsTest : IIntegrationTest
    {
        private readonly IGenericIntegrationProcessorOperationService<Order, ExportFlowOrdersInvoice> _integrationProcessorOperationService;

        public PerfomedOperationsProcessorOperationsTest(IGenericIntegrationProcessorOperationService<Order, ExportFlowOrdersInvoice> integrationProcessorOperationService)
        {
            _integrationProcessorOperationService = integrationProcessorOperationService;
        }

        public ITestResult Execute()
        {
            var flowDescription = new FlowDescription
                {
                    EntityName = EntityType.Instance.Order(),
                    FlowName = "flowOrders",
                    SchemaResourceName = "flowOrders_Invoice",
                    IntegrationEntityName = EntityType.Instance.ExportFlowOrdersInvoice()
                };

            //TestExportOperations(flowDescription);
            TestExportFailedEntities(flowDescription);

            return OrdinaryTestResult.As.Succeeded;
        }

        private void TestExportOperations(FlowDescription flowDescription)
        {
            var pendingOperations =
                GetPersistedPerformedBusinessOperations();
            //GetFakedPerformedBusinessOperations();

            _integrationProcessorOperationService.ExportOperations(flowDescription, pendingOperations, 10);
        }

        private void TestExportFailedEntities(FlowDescription flowDescription)
        {
            _integrationProcessorOperationService.ExportFailedEntities(flowDescription, 10);
        }

        private IEnumerable<PerformedBusinessOperation> GetPersistedPerformedBusinessOperations()
        {
            return _integrationProcessorOperationService.GetPendingOperations(10);
        }

        private IEnumerable<PerformedBusinessOperation> GetFakedPerformedBusinessOperations()
        {
            return new[] 
            { 
                new PerformedBusinessOperation
                {
                    Id = 733916,//200979784733154055,
                    Operation = 31,
                    Descriptor = -2081716307,
                    Context = "<context><entity change=\"3\" type=\"151\" id=\"733916\" /></context>",
                    Date = DateTime.UtcNow,
                    Parent = null
                } 
            };
        }
    }
}
