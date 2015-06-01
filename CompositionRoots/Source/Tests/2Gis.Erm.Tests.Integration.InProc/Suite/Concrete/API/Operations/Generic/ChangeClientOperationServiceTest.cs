using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ChangeClientOperationServiceTest : IIntegrationTest
    {
        private readonly IFinder _finder;
        private readonly IChangeGenericEntityClientService<Firm> _changeClientGenericEntityService;
        
        public ChangeClientOperationServiceTest(
            IFinder finder,
            IChangeGenericEntityClientService<Firm> changeClientGenericEntityService)
        {
            _finder = finder;
            _changeClientGenericEntityService = changeClientGenericEntityService;
        }

        public ITestResult Execute()
        {
            var targetFirm = 
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<Firm>() && FirmSpecs.Firms.Find.HasClient())
                    .Top();

            var targetClient = _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<Client>() && new FindSpecification<Client>(c => c.Id != targetFirm.ClientId))
                    .Top();

            return Result
                .When(_changeClientGenericEntityService.Execute(targetFirm.Id, targetClient.Id, true))
                .Then(result => result.Should().BeNull());
        }
    }
}