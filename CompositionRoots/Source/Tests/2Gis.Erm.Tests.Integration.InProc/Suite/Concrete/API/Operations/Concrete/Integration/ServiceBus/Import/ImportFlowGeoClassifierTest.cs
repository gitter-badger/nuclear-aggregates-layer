using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Integration.ServiceBus.Import
{
    public class ImportFlowGeoClassifierTest : IIntegrationTest
    {
        private readonly IImportFromServiceBusService _service;

        public ImportFlowGeoClassifierTest(IImportFromServiceBusService service)
        {
            _service = service;
        }

        public ITestResult Execute()
        {
            _service.Import("flowGeoClassifier");

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}