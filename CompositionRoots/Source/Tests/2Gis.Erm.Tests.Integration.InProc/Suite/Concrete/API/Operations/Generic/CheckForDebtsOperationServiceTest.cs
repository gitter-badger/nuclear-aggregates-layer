using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class CheckForDebtsOperationServiceTest : IIntegrationTest
    {
        private readonly IDebtProcessingSettings _debtProcessingSettings;
        private readonly IFinder _finder;
        private readonly ICheckGenericEntityForDebtsService<Account> _checkForDebtsGenericEntityService;

        public CheckForDebtsOperationServiceTest(
            IDebtProcessingSettings debtProcessingSettings,
            IFinder finder,
            ICheckGenericEntityForDebtsService<Account> checkForDebtsGenericEntityService)
        {
            _debtProcessingSettings = debtProcessingSettings;
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
                .First(x => x.NetDebt <= _debtProcessingSettings.MinDebtAmount);

            return Result
                .When(_checkForDebtsGenericEntityService.CheckForDebts(targetAccount.Id))
                .Then(result => result.DebtsExist.Should().BeTrue());
        }
    }
}