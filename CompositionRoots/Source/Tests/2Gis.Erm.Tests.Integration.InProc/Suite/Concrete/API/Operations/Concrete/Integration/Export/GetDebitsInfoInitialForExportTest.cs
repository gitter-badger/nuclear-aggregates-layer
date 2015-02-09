using System;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Integration.Export
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public class GetDebitsInfoInitialForExportTest : IIntegrationTest
    {
        private readonly IGetDebitsInfoInitialForExportOperationService _debitsInfoInitialForExportOperationService;
        private readonly ICommonLog _commonLog;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitProvider;
        private readonly IUseCaseTuner _useCaseTuner;

        public GetDebitsInfoInitialForExportTest(IGetDebitsInfoInitialForExportOperationService debitsInfoInitialForExportOperationService,
                                                 ICommonLog commonLog,
                                                 IAppropriateEntityProvider<OrganizationUnit> organizationUnitProvider,
                                                 IUseCaseTuner useCaseTuner)
        {
            _debitsInfoInitialForExportOperationService = debitsInfoInitialForExportOperationService;
            _commonLog = commonLog;
            _organizationUnitProvider = organizationUnitProvider;
            _useCaseTuner = useCaseTuner;
        }

        public ITestResult Execute()
        {
            _useCaseTuner.AlterDuration<GetDebitsInfoInitialForExportTest>();

            var startDate = new DateTime(2014, 11, 01);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);
            _debitsInfoInitialForExportOperationService.Get(startDate, endDate, new long[] { 1 });
            var allIds =
                _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>(), 200)
                                         .Select(x => x.Id);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _debitsInfoInitialForExportOperationService.Get(startDate, endDate, allIds);
            stopwatch.Stop();
            _commonLog.InfoFormatEx("Выборка списаний завершена за {0:F2}", stopwatch.Elapsed.TotalSeconds);

            return OrdinaryTestResult.As.Succeeded;
        }

        private static FindSpecification<OrganizationUnit> ByContributionType(ContributionTypeEnum type)
        {
            return new FindSpecification<OrganizationUnit>(x => x.BranchOfficeOrganizationUnits.Any(y => y.IsPrimary && y.BranchOffice.ContributionTypeId == (int)type));
        }
    }
}