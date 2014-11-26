using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Services.Notifications;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;
using MessageType = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "It's a test.")]
    class OrderProcessingRequestNotificationFormatterSpecs
    {
        private class Context
        {
            protected static IOrderProcessingRequestService OrderProcessingRequestService;
            protected static OrderProcessingRequestNotificationData OrderProcessingRequestNotificationData;
            protected static OrderProcessingRequestNotificationFormatter Target;
            protected static OrderProcessingRequest OrderProcessingRequest;
            protected static List<IMessageWithType> MessagesToSend;
            protected static NotificationMessage Result;

            private const long ORDER_PROCESSING_REQUEST_ID = 11111111111111;

            private Establish context = () =>
                {
                    OrderProcessingRequestService = SetupOrderProcessingRequestService();

                    Target = new OrderProcessingRequestNotificationFormatter(OrderProcessingRequestService);
                    OrderProcessingRequest = new OrderProcessingRequest { Id = ORDER_PROCESSING_REQUEST_ID };
                    MessagesToSend = new List<IMessageWithType>();
                };

            private Because of = () => Result = Target.Format(OrderProcessingRequest, MessagesToSend);

            private static IOrderProcessingRequestService SetupOrderProcessingRequestService()
            {
                var orderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();

                OrderProcessingRequestNotificationData = new OrderProcessingRequestNotificationData();

                Mock.Get(orderProcessingRequestService)
                    .Setup(x => x.GetNotificationData(ORDER_PROCESSING_REQUEST_ID))
                    .Returns(OrderProcessingRequestNotificationData);

                return orderProcessingRequestService;
            }

            protected static IMessageWithType CreateMessageWithType(string messageText, MessageType type)
            {
                return new MessageWithType { MessageText = messageText, Type = type };
            }
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_debug_message : Context
        {
            private const string DEBUG_MESSAGE = "DEBUG_MESSAGE";

            private Establish context = () => MessagesToSend.Add(CreateMessageWithType(DEBUG_MESSAGE, MessageType.Debug));

            private It should_be_ignored = () => Result.Body.Should().NotContain(DEBUG_MESSAGE);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_info_message : Context
        {
            private const string INFO_MESSAGE = "INFO_MESSAGE";

            private Establish context = () => MessagesToSend.Add(CreateMessageWithType(INFO_MESSAGE, MessageType.Info));

            private It should_be_ignored = () => Result.Body.Should().NotContain(INFO_MESSAGE);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_error_message : Context
        {
            private const string ERROR_MESSAGE = "ERROR_MESSAGE";

            private Establish context = () => MessagesToSend.Add(CreateMessageWithType(ERROR_MESSAGE, MessageType.Error));

            private It should_be_insert_into_message_body = () => Result.Body.Should().Contain(ERROR_MESSAGE);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_warning_message : Context
        {
            private const string WARNING_MESSAGE = "WARNING_MESSAGE";

            private Establish context = () => MessagesToSend.Add(CreateMessageWithType(WARNING_MESSAGE, MessageType.Warning));

            private It should_be_insert_into_message_body = () => Result.Body.Should().Contain(WARNING_MESSAGE);
        }

        private abstract class DataContext : Context
        {
            protected const string FIRM_NAME = "FIRM_NAME";
            protected const string LEGAL_PERSON_NAME = "LEGAL_PERSON_NAME";
            protected const string REQUEST_DESCRIPTION = "REQUEST_DESCRIPTION";

            protected const int RELEASE_COUNT_PLAN = 8;
            protected const string BEGIN_DISTRIBUTION_DATE = "01.11.2013";

            protected static string ExpectedSubject;
            protected static string ExpectedBody;

            private Establish context = () =>
                {
                    OrderProcessingRequestNotificationData.FirmName = FIRM_NAME;
                    OrderProcessingRequestNotificationData.LegalPersonName = LEGAL_PERSON_NAME;

                    OrderProcessingRequest.BeginDistributionDate = DateTime.Parse(BEGIN_DISTRIBUTION_DATE);
                    OrderProcessingRequest.ReleaseCountPlan = RELEASE_COUNT_PLAN;
                    OrderProcessingRequest.Description = REQUEST_DESCRIPTION;
                };
        }

        private abstract class CreateOrderContext : DataContext
        {
            private Establish context = () =>
                {
                    OrderProcessingRequest.RequestType = OrderProcessingRequestType.CreateOrder;
                    ExpectedSubject = BLResources.OrderProcessingRequestCreationResultEmailSubject;

                    var header = BLResources.OrderProcessingRequestCreationResultEmailHeader;

                    var requestData = string.Format(BLResources.OrderProcessingRequestResultEmailDataTemplate,
                                                    FIRM_NAME,
                                                    LEGAL_PERSON_NAME,
                                                    BEGIN_DISTRIBUTION_DATE,
                                                    RELEASE_COUNT_PLAN);

                    var description = string.Format(BLResources.OrderProcessingRequestResultEmailDescriptionTemplate, REQUEST_DESCRIPTION);

                    ExpectedBody = string.Format("{1}{0}{0}{2}{0}{0}{3}", Environment.NewLine, header, requestData, description);
                };
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_formatting_create_order_request_without_messages : CreateOrderContext
        {
            private Establish context = () =>
                {
                    ExpectedBody = string.Format("{1}{0}{0}{2}",
                                                 Environment.NewLine,
                                                 ExpectedBody,
                                                 BLResources.OrderProcessingRequestCreationResultEmailFooter);
                };

            private It should_returns_expected_subject = () => Result.Subject.Should().Be(ExpectedSubject);

            private It should_returns_expected_body = () => Result.Body.Should().Be(ExpectedBody);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_formatting_create_order_request_with_messages : CreateOrderContext
        {
            private const string MESSAGE_TEXT_1 = "MESSAGE_TEXT_1";
            private const string MESSAGE_TEXT_2 = "MESSAGE_TEXT_2";

            private const string RENEWED_ORDER_NUMBER = "RENEWED_ORDER_NUMBER";

            private Establish context = () =>
            {
                // якобы заявка обработана
                OrderProcessingRequest.RenewedOrderId = 1; 
                OrderProcessingRequestNotificationData.RenewedOrderNumber = RENEWED_ORDER_NUMBER;

                MessagesToSend.Add(CreateMessageWithType(MESSAGE_TEXT_1, MessageType.Warning));
                MessagesToSend.Add(CreateMessageWithType(MESSAGE_TEXT_2, MessageType.Error));

                var resultText = string.Format(BLResources.OrderProcessingRequestResultEmailRenewedOrderTemplate,
                                               RENEWED_ORDER_NUMBER);

                var errorsText = string.Format(BLResources.OrderProcessingRequestResultEmailErrorsEnumerationTemplate,
                                               "1. " + MESSAGE_TEXT_1 + Environment.NewLine +
                                               "2. " + BLResources.OrderProcessingRequestResultEmailErrorMessagePrefixTemplate + MESSAGE_TEXT_2);

                ExpectedBody = string.Format("{1}{0}{0}{2}{0}{0}{3}", Environment.NewLine, ExpectedBody, resultText, errorsText);
            };

            private It should_returns_expected_subject = () => Result.Subject.Should().Be(ExpectedSubject);

            private It should_returns_expected_body = () => Result.Body.Should().Be(ExpectedBody);
        }

        private class ProlongateOrderContext : DataContext
        {
            protected const string BASE_ORDER_NUMBER = "BASE_ORDER_NUMBER";

            private Establish context = () =>
                {
                    OrderProcessingRequest.RequestType = OrderProcessingRequestType.ProlongateOrder;
                    OrderProcessingRequestNotificationData.BaseOrderNumber = BASE_ORDER_NUMBER;

                    ExpectedSubject = BLResources.OrderProcessingRequestProlongationResultEmailSubject;

                    var header = string.Format(BLResources.OrderProcessingRequestProlongationResultEmailHeaderTemplate, BASE_ORDER_NUMBER);

                    var requestData = string.Format(BLResources.OrderProcessingRequestResultEmailDataTemplate,
                                                    FIRM_NAME,
                                                    LEGAL_PERSON_NAME,
                                                    BEGIN_DISTRIBUTION_DATE,
                                                    RELEASE_COUNT_PLAN);

                    var description = string.Format(BLResources.OrderProcessingRequestResultEmailDescriptionTemplate, REQUEST_DESCRIPTION);

                    ExpectedBody = string.Format("{1}{0}{0}{2}{0}{0}{3}", Environment.NewLine, header, requestData, description);
                };
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_formatting_prolongate_order_request_without_messages : ProlongateOrderContext
        {
            private It should_returns_expected_subject = () => Result.Subject.Should().Be(ExpectedSubject);

            private It should_returns_expected_body = () => Result.Body.Should().Be(ExpectedBody);
        }

        [Tags("BL")]
        [Subject(typeof(OrderProcessingRequestNotificationFormatter))]
        private class When_formatting_prolongate_order_request_with_messages : ProlongateOrderContext
        {
            private const string MESSAGE_TEXT_1 = "MESSAGE_TEXT_1";
            private const string MESSAGE_TEXT_2 = "MESSAGE_TEXT_2";

            private const string RENEWED_ORDER_NUMBER = "RENEWED_ORDER_NUMBER";

            private Establish context = () =>
            {
                // якобы заявка обработана
                OrderProcessingRequest.RenewedOrderId = 1;
                OrderProcessingRequestNotificationData.RenewedOrderNumber = RENEWED_ORDER_NUMBER;

                MessagesToSend.Add(CreateMessageWithType(MESSAGE_TEXT_1, MessageType.Warning));
                MessagesToSend.Add(CreateMessageWithType(MESSAGE_TEXT_2, MessageType.Error));

                var resultText = string.Format(BLResources.OrderProcessingRequestResultEmailRenewedOrderTemplate,
                                               RENEWED_ORDER_NUMBER);

                var errorsText = string.Format(BLResources.OrderProcessingRequestResultEmailErrorsEnumerationTemplate,
                                               "1. " + MESSAGE_TEXT_1 + Environment.NewLine +
                                               "2. " + BLResources.OrderProcessingRequestResultEmailErrorMessagePrefixTemplate + MESSAGE_TEXT_2);

                ExpectedBody = string.Format("{1}{0}{0}{2}{0}{0}{3}", Environment.NewLine, ExpectedBody, resultText, errorsText);
            };

            private It should_returns_expected_subject = () => Result.Subject.Should().Be(ExpectedSubject);

            private It should_returns_expected_body = () => Result.Body.Should().Be(ExpectedBody);
        }
    }
}
