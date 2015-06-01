using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ChangeTerritoryOperationServiceTest : IIntegrationTest
    {
        private readonly IFinder _finder;
        private readonly IChangeGenericEntityTerritoryService<Firm> _changeEntityTerritoryService;

        public ChangeTerritoryOperationServiceTest(
            IFinder finder,
            IChangeGenericEntityTerritoryService<Firm> changeEntityTerritoryService)
        {
            _finder = finder;
            _changeEntityTerritoryService = changeEntityTerritoryService;
        }

        public ITestResult Execute()
        {
            var targetFirm = 
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<Firm>())
                    .Top();

            var targetTerritory = 
                _finder
                    .Find(Specs.Find.Active<Territory>() && new FindSpecification<Territory>(t => t.Id != targetFirm.TerritoryId))
                    .Top();

            return Result
                .When(() => _changeEntityTerritoryService.ChangeTerritory(targetFirm.Id, targetTerritory.Id))
                .Then(result => result.Status.Should().Be(TestResultStatus.Succeeded));
        }
    }
}