using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Integration.Export
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public class GetAccountDetailsForExportContentTest : IIntegrationTest
    {
        private readonly IGetDebitsInfoInitialForExportOperationService _debitsInfoInitialForExportOperationService;
        private readonly IUseCaseTuner _useCaseTuner;

        public GetAccountDetailsForExportContentTest(IGetDebitsInfoInitialForExportOperationService debitsInfoInitialForExportOperationService,
                                                 IUseCaseTuner useCaseTuner)
        {
            _debitsInfoInitialForExportOperationService = debitsInfoInitialForExportOperationService;
            _useCaseTuner = useCaseTuner;
        }

        public ITestResult Execute()
        {
            _useCaseTuner.AlterDuration<GetDebitsInfoInitialForExportTest>();

            var startDate = new DateTime(2014, 11, 01);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);
            _debitsInfoInitialForExportOperationService.Get(startDate, endDate, new long[] { 67 });

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}