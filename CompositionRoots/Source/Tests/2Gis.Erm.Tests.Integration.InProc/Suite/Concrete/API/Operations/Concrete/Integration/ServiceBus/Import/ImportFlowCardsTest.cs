using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Integration.ServiceBus.Import
{
    public class ImportFlowCardsTest : IIntegrationTest
    {
        private readonly IImportFromServiceBusService _service;

        public ImportFlowCardsTest(IImportFromServiceBusService service)
        {
            _service = service;
        }

        public ITestResult Execute()
        {
            _service.Import("flowCards");

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}