using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage;
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
                    .First();

            var targetTerritory = 
                _finder
                    .Find(Specs.Find.Active<Territory>() && new FindSpecification<Territory>(t => t.Id != targetFirm.TerritoryId))
                    .First();

            return Result
                .When(() => _changeEntityTerritoryService.ChangeTerritory(targetFirm.Id, targetTerritory.Id))
                .Then(result => result.Status.Should().Be(TestResultStatus.Succeeded));
        }
    }
}