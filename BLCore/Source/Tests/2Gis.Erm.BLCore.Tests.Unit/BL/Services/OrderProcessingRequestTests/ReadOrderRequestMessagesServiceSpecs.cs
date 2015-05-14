using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Security.API.UserContext.Identity;
using NuClear.Storage;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.OrderProcessingRequestTests
{
    public class ReadOrderRequestMessagesServiceSpecs
    {
        [Subject(typeof(ReadOrderRequestMessagesService))]
        private abstract class MockContext
        {
            protected const long defaultRequestId = 1;

            protected static OrderProcessingRequestMessage[] RequestMessages { get; set; }
            protected static ReadOrderRequestMessagesService ReadOrderRequestMessagesService { get; set; }
            protected static IEnumerable<RequestMessageDetailDto> Result { get; set; }

            private static IFinder Finder { get; set; }
            private static ISecurityServiceUserIdentifier SecurityServiceUserIdentifier { get; set; }

            private Establish context = () =>
                {
                    var finderMock = new Mock<IFinder>();
                    finderMock.Setup(x => x.Find(Moq.It.IsAny<IFindSpecification<OrderProcessingRequestMessage>>()))
                              .Returns(
                                  (IFindSpecification<OrderProcessingRequestMessage> x) => RequestMessages.AsQueryable().Where(x.Predicate));

                    Finder = finderMock.Object;

                    var securityServiceUserIdentifierMock = new Mock<ISecurityServiceUserIdentifier>();
                    securityServiceUserIdentifierMock.Setup(x => x.GetUserInfo(Moq.It.IsAny<long?>())).Returns(new UserInfo(0, "Chapa", "Чапик"));
                    SecurityServiceUserIdentifier = securityServiceUserIdentifierMock.Object;


                    ReadOrderRequestMessagesService = new ReadOrderRequestMessagesService(Finder, SecurityServiceUserIdentifier);
                };

            protected static OrderProcessingRequestMessage GetRequestMessage()
            {
                return new OrderProcessingRequestMessage
                    {
                        Id = 1,
                        OrderRequestId = defaultRequestId,
                        IsActive = true,
                        MessageType = 0,
                        MessageParameters = "<Parameters><Parameter>Test</Parameter></Parameters>"
                    };
            }
        }

        private class When_service_reads_messages : MockContext
        {
            private const long activeMessageId = 1;
            private const long inactiveMessageId = 2;
            private const long someOtherRequestMessageId = 3;
            private const long someOtherRequestId = 3;

            private Establish context = () =>
                {
                    RequestMessages = new[]
                        {
                            GetRequestMessage(),
                            GetRequestMessage(),
                            GetRequestMessage(),
                        };

                    RequestMessages[0].IsActive = true;
                    RequestMessages[0].Id = activeMessageId;

                    RequestMessages[1].IsActive = false;
                    RequestMessages[1].Id = inactiveMessageId;

                    RequestMessages[1].Id = someOtherRequestMessageId;
                    RequestMessages[1].OrderRequestId = someOtherRequestId;
                };

            private Because of = () => Result = ReadOrderRequestMessagesService.GetRequestMessages(defaultRequestId);

            private It shouldnt_take_inactive_ones = () => Result.Should().NotContain(x => x.Id == inactiveMessageId);

            private It shouldnt_take_messages_for_unspecified_request =
                () => Result.Should().NotContain(x => x.RequestId == someOtherRequestId);

            private It should_take_all_messages_for_specified_request =
                () =>
                Result.Select(x => x.Id)
                      .Should()
                      .BeEquivalentTo(RequestMessages.Where(x => x.IsActive && x.OrderRequestId == defaultRequestId).Select(x => x.Id).ToArray());
        }
    }
}
