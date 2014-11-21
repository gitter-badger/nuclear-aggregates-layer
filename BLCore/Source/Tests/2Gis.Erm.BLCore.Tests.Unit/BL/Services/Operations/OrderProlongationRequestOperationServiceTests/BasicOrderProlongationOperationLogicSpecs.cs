using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;
using MessageType = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test.")]
    public class BasicOrderProlongationOperationLogicSpecs
    {
        private class Context
        {
            #region Fields

            protected const long ID = 1111111111110;
            protected const long BASE_ORDER_ID = 1111111111111;
            protected const long CLIENT_ID = 211111111111;
            protected const long FIRM_ID = 311111111111;
            protected const long USER_ID = 411111111111111;

            protected static IOrderProcessingRequestService OrderProcessingRequestService;
            protected static IOperationScopeFactory ScopeFactory;
            protected static ICopyOrderOperationService CopyOrderOperationService;
            protected static IRepairOutdatedPositionsOperationService RepairOutdatedPositionsOperationService;
            protected static IObtainDealForBizzacountOrderOperationService ObtainDealForBizzacountOrderOperationService;
            protected static IOrderProcessingRequestEmailSender EmailSender;
            protected static IOrderProcessingOwnerSelectionService OrderProcessingOwnerSelectionService;

            protected static BasicOrderProlongationOperationLogic Target;
            protected static OrderProcessingResult Result;
            protected static Exception CatchedException;

            protected static OrderProcessingRequest OrderProcessingRequest;

            protected static Order BaseOrder;
            protected static OrderProcessingRequestFirmDto FirmDto;
            protected static ObtainDealForBizzacountOrderResult RepairDealResult;
            protected static CopyOrderResult CopyOrderResult;
            protected static User Owner;
            protected static IEnumerable<IMessageWithType> MessagesToSend;
            protected static IEnumerable<IMessageWithType> MessagesToSave;
            #endregion

            private Establish context = () =>
                {
                    OrderProcessingRequestService = SetupOrderProcessingRequestService();
                    ScopeFactory = SetupScopeFactory();
                    CopyOrderOperationService = SetupCopyOrderOperationService();
                    RepairOutdatedPositionsOperationService = SetupRepairOutdatedPositionsOperationService();
                    ObtainDealForBizzacountOrderOperationService = SetupObtainDealForBizzacountOrderOperationService();
                    EmailSender = SetupEmailSender();
                    OrderProcessingOwnerSelectionService = SetupOrderProcessingOwnerSelectionService();

                    Target = new BasicOrderProlongationOperationLogic(
                        OrderProcessingRequestService, 
                        ScopeFactory, 
                        CopyOrderOperationService,
                        RepairOutdatedPositionsOperationService,
                        ObtainDealForBizzacountOrderOperationService, 
                        OrderProcessingOwnerSelectionService, 
                        EmailSender);

                    BaseOrder = new Order { Id = BASE_ORDER_ID };
                    OrderProcessingRequest = new OrderProcessingRequest
                        {
                            Id = ID,
                            FirmId = FIRM_ID,
                            BaseOrder = BaseOrder, 
                            BaseOrderId = BASE_ORDER_ID,
                            RequestType = (int)OrderProcessingRequestType.ProlongateOrder,
                            BeginDistributionDate = DateTime.Today
                        };
                };

            #region SetupMock

            private static IOrderProcessingRequestEmailSender SetupEmailSender()
            {
                var emailSender = Mock.Of<IOrderProcessingRequestEmailSender>();
                var sendResult = new OrderProcessingRequestEmailSendResult();

                Mock.Get(emailSender)
                    .Setup(x => x.SendProcessingMessages(Moq.It.IsAny<OrderProcessingRequest>(), Moq.It.IsAny<IEnumerable<IMessageWithType>>()))
                    .Returns<OrderProcessingRequest, IEnumerable<IMessageWithType>>((x, y) =>
                    {
                        MessagesToSend = y;
                        return sendResult;
                    });

                return emailSender;
            }

            private static IOrderProcessingOwnerSelectionService SetupOrderProcessingOwnerSelectionService()
            {
                var orderProcessingOwnerSelectionService = Mock.Of<IOrderProcessingOwnerSelectionService>();

                Owner = new User { Id = USER_ID };

                Mock.Get(orderProcessingOwnerSelectionService)
                    .Setup(x => x.FindOwner(Moq.It.IsAny<OrderProcessingRequest>(), Moq.It.IsAny<ICollection<IMessageWithType>>()))
                    .Returns(Owner);

                return orderProcessingOwnerSelectionService;
            }

            private static IObtainDealForBizzacountOrderOperationService SetupObtainDealForBizzacountOrderOperationService()
            {
                var obtainDealForBizzacountOrderOperationService = Mock.Of<IObtainDealForBizzacountOrderOperationService>();

                RepairDealResult = new ObtainDealForBizzacountOrderResult();

                Mock.Get(obtainDealForBizzacountOrderOperationService)
                        .Setup(x => x.ObtainDealForOrder(Moq.It.IsAny<long>(), Moq.It.IsAny<long>()))
                        .Returns(RepairDealResult);

                return obtainDealForBizzacountOrderOperationService;
            }

            private static IRepairOutdatedPositionsOperationService SetupRepairOutdatedPositionsOperationService()
            {
                return Mock.Of<IRepairOutdatedPositionsOperationService>();
            }

            private static ICopyOrderOperationService SetupCopyOrderOperationService()
            {
                var copyOrderOperationService = Mock.Of<ICopyOrderOperationService>();

                CopyOrderResult = new CopyOrderResult();

                Mock.Get(copyOrderOperationService)
                    .Setup(x => x.CopyOrder(Moq.It.IsAny<long>(),
                                    Moq.It.IsAny<DateTime>(),
                                    Moq.It.IsAny<short>(),
                                    Moq.It.IsAny<DiscountType>(),
                                    Moq.It.IsAny<long>()))
                    .Returns(CopyOrderResult);

                return copyOrderOperationService;
            }

            private static IOperationScopeFactory SetupScopeFactory()
            {
                var scopeFactory = Mock.Of<IOperationScopeFactory>();

                var scope = Mock.Of<IOperationScope>();
                Mock.Get(scopeFactory)
                    .Setup(x => x.CreateNonCoupled<ProlongateOrderByRequestIdentity>())
                    .Returns(scope);

                Mock.Get(scope).Setup(x => x.Added<Order>(Moq.It.IsAny<long>())).Returns(scope);
                Mock.Get(scope).Setup(x => x.Updated<OrderProcessingRequest>(Moq.It.IsAny<long>())).Returns(scope);

                return scopeFactory;
            }

            private static IOrderProcessingRequestService SetupOrderProcessingRequestService()
            {
                var orderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();
                FirmDto = new OrderProcessingRequestFirmDto { Client = new OrderProcessingRequestFirmDto.ClientDto() };

                Mock.Get(orderProcessingRequestService)
                      .Setup(x => x.SaveMessagesForOrderProcessingRequest(Moq.It.IsAny<long>(), Moq.It.IsAny<IEnumerable<IMessageWithType>>()))
                      .Callback<long, IEnumerable<IMessageWithType>>((x, y) =>
                      {
                          MessagesToSave = y;
                      });

                Mock.Get(orderProcessingRequestService)
                    .Setup(x => x.GetOrderDto(BASE_ORDER_ID))
                    .Returns(() => new OrderProcessingRequestOrderDto { Id = BASE_ORDER_ID, OwnerCode = CLIENT_ID, Number = "XYZ" });

                Mock.Get(orderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(FIRM_ID))
                    .Returns(FirmDto);

                return orderProcessingRequestService;
            }

            #endregion
        }

        [Tags("BL")]
        [Subject(typeof(BasicOrderProlongationOperationLogic))]
        private class When_base_order_missing : Context
        {
            private Establish context = () => OrderProcessingRequest.BaseOrderId = null;

            private Because of = () => CatchedException = Catch.Exception(() => Target.ExecuteRequest(OrderProcessingRequest));

            private It should_throwns_InvalidOperationException = () => CatchedException.Should().BeOfType<InvalidOperationException>();

            private It should_send_error_message = () => MessagesToSend.Select(x => x.Type).Single().Should().Be(MessageType.Error);

            private It should_save_error_message = () => MessagesToSave.Select(x => x.Type).Single().Should().Be(MessageType.Error);
        }

        [Tags("BL")]
        [Subject(typeof(BasicOrderProlongationOperationLogic))]
        private class When_begin_destribution_date_is_in_past : Context
        {
            private Establish context = () => OrderProcessingRequest.BeginDistributionDate = DateTime.Today.AddDays(-1);

            private Because of = () => CatchedException = Catch.Exception(() => Target.ExecuteRequest(OrderProcessingRequest));

            private It should_throwns_InvalidOperationException = () => CatchedException.Should().BeOfType<BusinessLogicException>();

            private It should_send_error_message = () => MessagesToSend.Select(x => x.Type).Single().Should().Be(MessageType.Error);

            private It should_save_error_message = () => MessagesToSave.Select(x => x.Type).Single().Should().Be(MessageType.Error);
        }

        [Tags("BL")]
        [Subject(typeof(BasicOrderProlongationOperationLogic))]
        private class When_prolongation : Context
        {
            private const long COPIED_ORDER_ID = 2222222222222222;
            private const string COPIED_ORDER_NUMBER = "TEST_COPIED_ORDER_NUMBER";

            private Establish context = () =>
            {
                    CopyOrderResult.OrderId = COPIED_ORDER_ID;
                    CopyOrderResult.OrderNumber = COPIED_ORDER_NUMBER;
            };

            private Because of = () => Result = Target.ExecuteRequest(OrderProcessingRequest);

            private It should_find_owner = () => Mock.Get(OrderProcessingOwnerSelectionService)
                                                     .Verify(x => x.FindOwner(OrderProcessingRequest, Moq.It.IsAny<ICollection<IMessageWithType>>()), Times.Once);

            private It should_obtain_deal_for_order = () => Mock.Get(ObtainDealForBizzacountOrderOperationService)
                                                                .Verify(x => x.ObtainDealForOrder(BASE_ORDER_ID, USER_ID), Times.Once);

            private It should_repair_outdated_positions = () => Mock.Get(RepairOutdatedPositionsOperationService)
                                                                    .Verify(x => x.RepairOutdatedPositions(COPIED_ORDER_ID, true), Times.Once);

            private It should_set_order_processing_request_renewed_order_id = () => OrderProcessingRequest.RenewedOrderId.Should().Be(COPIED_ORDER_ID);

            private It should_complete_order_processing_request = () => OrderProcessingRequest.State.Should().Be((int)OrderProcessingRequestState.Completed);

            private It should_update_persistance_order_processing_request = () => Mock.Get(OrderProcessingRequestService)
                                                                                      .Verify(x => x.Update(OrderProcessingRequest), Times.Once);

            private It should_send_processing_messages = () => Mock.Get(EmailSender)
                                                                   .Verify(x => x.SendProcessingMessages(OrderProcessingRequest, Moq.It.IsAny<ICollection<IMessageWithType>>()), Times.Once);

            private It should_save_messages = () => Mock.Get(OrderProcessingRequestService)
                .Verify(x => x.SaveMessagesForOrderProcessingRequest(OrderProcessingRequest.Id, Moq.It.IsAny<ICollection<IMessageWithType>>()), Times.Once);

            private It should_returns_result_without_messages = () => Result.Messages.Should().BeEmpty();

            private It should_returns_result_with_new_OrderId = () => Result.OrderId.Should().Be(COPIED_ORDER_ID);

            private It should_returns_result_with_new_OrderNumber = () => Result.OrderNumber.Should().Be(COPIED_ORDER_NUMBER);

            private It should_send_success_message = () => MessagesToSend.Select(x => x.Type).Single().Should().Be(MessageType.Info);

            private It should_save_success_message = () => MessagesToSave.Select(x => x.Type).Single().Should().Be(MessageType.Info);
        }
    }
}