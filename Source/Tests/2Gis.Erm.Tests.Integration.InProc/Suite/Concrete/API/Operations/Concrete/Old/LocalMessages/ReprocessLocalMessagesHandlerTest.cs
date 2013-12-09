using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LocalMessages
{
    public class ReprocessLocalMessagesHandlerTest
    {
        private readonly IPublicService _publicService;

        public ReprocessLocalMessagesHandlerTest(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public ITestResult Execute()
        {
            return Result.When(_publicService.Handle(new ReprocessLocalMessagesRequest()))
                         .Then(r => r.Should().NotBeNull());
        }
    }
}