using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class CheckForDebtsOperationServiceTest : IIntegrationTest
    {
        private readonly IAppSettings _appSettings;
        private readonly IFinder _finder;
        private readonly ICheckGenericEntityForDebtsService<Account> _checkForDebtsGenericEntityService;

        public CheckForDebtsOperationServiceTest(
            IAppSettings appSettings,
            IFinder finder,
            ICheckGenericEntityForDebtsService<Account> checkForDebtsGenericEntityService)
        {
            _appSettings = appSettings;
            _finder = finder;
            _checkForDebtsGenericEntityService = checkForDebtsGenericEntityService;
        }

        public ITestResult Execute()
        {
            var targetAccount = _finder
                .Find(Specs.Find.ActiveAndNotDeleted<Account>())
                .Select(a => new
                    {
                        a.Id,
                        NetDebt = a.Balance - (a.Locks
                                                .Where(l => l.IsActive && !l.IsDeleted)
                                                .Sum(l => (decimal?)l.PlannedAmount) ?? 0)
                    })
                .First(x => x.NetDebt <= _appSettings.MinDebtAmount);

            return Result
                .When(_checkForDebtsGenericEntityService.CheckForDebts(targetAccount.Id))
                .Then(result => result.DebtsExist.Should().BeTrue());
        }
    }
}