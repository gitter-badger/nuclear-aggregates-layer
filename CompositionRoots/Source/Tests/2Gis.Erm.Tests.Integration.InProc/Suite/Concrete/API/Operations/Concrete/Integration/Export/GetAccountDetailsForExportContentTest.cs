using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Integration.Export
{
    public class GetAccountDetailsForExportContentTest : IIntegrationTest
    {
        private readonly IGetDebitsInfoInitialForExportOperationService _debitsInfoInitialForExportOperationService;

        public GetAccountDetailsForExportContentTest(IGetDebitsInfoInitialForExportOperationService debitsInfoInitialForExportOperationService)
        {
            _debitsInfoInitialForExportOperationService = debitsInfoInitialForExportOperationService;
        }

        public ITestResult Execute()
        {
            var startDate = new DateTime(2014, 11, 01);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);
            _debitsInfoInitialForExportOperationService.Get(startDate, endDate, new long[] { 67 });

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}