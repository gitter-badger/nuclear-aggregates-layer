using DoubleGis.Erm.BL.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LocalMessages
{
    public class ProcessLocalMessagesHandlerTest : IIntegrationTest
    {
        private readonly IPublicService _publicService;

        public ProcessLocalMessagesHandlerTest(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public ITestResult Execute()
        {
            return Result.When(_publicService.Handle(new ProcessLocalMessagesRequest()))
                         .Then(r => r.Should().NotBeNull());
        }
    }
}