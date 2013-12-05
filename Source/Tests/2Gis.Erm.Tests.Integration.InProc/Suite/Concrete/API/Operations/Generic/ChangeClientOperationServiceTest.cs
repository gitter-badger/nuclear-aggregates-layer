using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BL.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

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
                    .First();

            var targetClient = _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<Client>() && new FindSpecification<Client>(c => c.Id != targetFirm.ClientId))
                    .First();

            return Result
                .When(_changeClientGenericEntityService.Execute(targetFirm.Id, targetClient.Id, true))
                .Then(result => result.Should().BeNull());
        }
    }
}