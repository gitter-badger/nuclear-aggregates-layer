using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Territories
{
    public class SelectCurrentUserTerritoriesExpressionHandlerTest : IIntegrationTest
    {
        private readonly IPublicService _publicService;

        public SelectCurrentUserTerritoriesExpressionHandlerTest(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public ITestResult Execute()
        {
            return Result.When(_publicService.Handle(new SelectCurrentUserTerritoriesExpressionRequest()))
                         .Then(r => r.Should().BeOfType<SelectCurrentUserTerritoriesExpressionResponse>());
        }
    }
}