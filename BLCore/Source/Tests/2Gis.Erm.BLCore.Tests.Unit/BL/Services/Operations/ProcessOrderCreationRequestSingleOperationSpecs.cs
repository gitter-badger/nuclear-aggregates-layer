using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using It = Machine.Specifications.It;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable MemberCanBePrivate.Local
namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    public static class ProcessOrderCreationRequestSingleOperationSpecs
    {
        private const long OrderCreationRequestId = 1;
        private const long ClientId = 2;
        private const long ClientOwnerCode = 3;
        private const long FirmOrganizationUnitId = 4;
        private const long SourceOrganizationUnitId = 5;
        private const long DealId = 6;
        private const long LegalPersonId = 7;
        private const long BranchOfficeOrganizationUnitId = 8;
        private const long AccountId = 9;
        private const int ReleaseCountPlanValid = 10;
        
        private static readonly DateTime BeginDistributionDate = new DateTime(2013, 01, 01);

        #region Setup routines
        private static IUserContext SetupUserContext()
        {
            var userContext = Mock.Of<IUserContext>();

            Mock.Get(userContext)
                .SetupGet(x => x.Identity)
                .Returns(new ErmUserIdentity(UserInfo.Empty));

            return userContext;
        }

        private static IOperationScopeFactory SetupOperationScopeFactory()
        {
            var operationScopeFactory = Mock.Of<IOperationScopeFactory>();

            Mock.Get(operationScopeFactory)
                .Setup(x => x.CreateNonCoupled<CreateOrderByRequestIdentity>())
                .Returns(Mock.Of<IOperationScope>);

            return operationScopeFactory;
        }

        private static IAccountReadModel SetupAccountReadModel()
        {
            var orderProcessingRequestAccountService = Mock.Of<IAccountReadModel>();

            Mock.Get(orderProcessingRequestAccountService)
                .Setup(x => x.FindPrimaryBranchOfficeOrganizationUnit(Moq.It.IsAny<long>()))
                .Returns<long>(id => new BranchOfficeOrganizationUnit { Id = BranchOfficeOrganizationUnitId, OrganizationUnitId = id });

            Mock.Get(orderProcessingRequestAccountService)
                .Setup(x => x.FindAccount(Moq.It.IsAny<long>(), Moq.It.IsAny<long>()))
                .Returns(new Account { Id = AccountId });

            return orderProcessingRequestAccountService;
        }

        private static IModifyBusinessModelEntityService<Order> SetupModifyOrderService()
        {
            var modifyOrderService = Mock.Of<IModifyBusinessModelEntityService<Order>>();
            return modifyOrderService;
        }

        private static IOrderProcessingRequestService SetupOrderProcessingRequestService()
        {
            var orderProcessingRequestService = Mock.Of<IOrderProcessingRequestService>();

            Mock.Get(orderProcessingRequestService)
                .Setup(x => x.GetProlongationRequestToProcess(Moq.It.IsAny<long>()))
                .Returns(new OrderProcessingRequest
                    {
                        Id = OrderCreationRequestId,
                        IsActive = true,
                        IsDeleted = false,
                        SourceOrganizationUnitId = SourceOrganizationUnitId,
                        LegalPersonId = LegalPersonId,
                        BeginDistributionDate = BeginDistributionDate,
                        ReleaseCountPlan = ReleaseCountPlanValid,
                        RequestType = OrderProcessingRequestType.CreateOrder,
                    });

            Mock.Get(orderProcessingRequestService)
                .Setup(x => x.GetFirmDto(Moq.It.IsAny<long>()))
                .Returns(new OrderProcessingRequestFirmDto
                    {
                        Client = new OrderProcessingRequestFirmDto.ClientDto
                            {
                                Id = ClientId,
                                OwnerCode = ClientOwnerCode,
                            },
                        OrganizationUnitId = FirmOrganizationUnitId
                    });

            return orderProcessingRequestService;
        }

        private static IOrderProcessingOwnerSelectionService SetupOrderProcessingOwnerSelectionService()
        {
            var orderProcessingRequestOwnerSelectionService = Mock.Of<IOrderProcessingOwnerSelectionService>();

            Mock.Get(orderProcessingRequestOwnerSelectionService)
                .Setup(x => x.FindOwner(Moq.It.IsAny<OrderProcessingRequest>(), Moq.It.IsAny<ICollection<IMessageWithType>>()))
                .Returns(new User());

            return orderProcessingRequestOwnerSelectionService;
        }

        private static IObtainDealForBizzacountOrderOperationService SetupObtainDealOperationService()
        {
            var repairDealForBizzacountOrderOperationService = Mock.Of<IObtainDealForBizzacountOrderOperationService>();

            Mock.Get(repairDealForBizzacountOrderOperationService)
                .Setup(x => x.CreateDealForClient(Moq.It.IsAny<long>(), Moq.It.IsAny<long>()))
                .Returns(new ObtainDealForBizzacountOrderResult { DealId = DealId });

            return repairDealForBizzacountOrderOperationService;
        }

        private static IClientDealSelectionService SetupOwnerSelectionService()
        {
            var clientDealSelectionService = Mock.Of<IClientDealSelectionService>();

            return clientDealSelectionService;
        }
        #endregion

        private abstract class BasicContext
        {
            protected static IOperationScopeFactory OperationScopeFactory;
            protected static IAccountReadModel AccountReadModel;
            protected static IModifyBusinessModelEntityService<Order> ModifyOrderService;
            protected static IOrderProcessingRequestService OrderProcessingRequestService;
            protected static IOrderProcessingOwnerSelectionService OrderProcessingOwnerSelectionService;
            protected static IObtainDealForBizzacountOrderOperationService ObtainDealForBizzacountOrderOperationService;
            protected static IClientDealSelectionService ClientDealSelectionService;
            protected static ProcessOrderCreationRequestSingleOperation Operation;
            protected static Exception ExceptionThrown;

            protected static OrderCreationResult OperationResult;

            private Establish context = () =>
                {
                    OperationScopeFactory = SetupOperationScopeFactory();
                    AccountReadModel = SetupAccountReadModel();
                    ModifyOrderService = SetupModifyOrderService();
                    OrderProcessingRequestService = SetupOrderProcessingRequestService();
                    OrderProcessingOwnerSelectionService = SetupOrderProcessingOwnerSelectionService();
                    ObtainDealForBizzacountOrderOperationService = SetupObtainDealOperationService();
                    ClientDealSelectionService = SetupOwnerSelectionService();

                    Operation = new ProcessOrderCreationRequestSingleOperation(OperationScopeFactory,
                                                                               AccountReadModel,
                                                                               ClientDealSelectionService,
                                                                               ModifyOrderService,
                                                                               OrderProcessingRequestService,
                                                                               OrderProcessingOwnerSelectionService,
                                                                               ObtainDealForBizzacountOrderOperationService);
                };

            private Because of = () =>
                {
                    OperationResult = null;
                    ExceptionThrown = null;

                    try
                    {
                        OperationResult = Operation.ProcessSingle(OrderCreationRequestId);
                    }
                    catch (Exception ex)
                    {
                        ExceptionThrown = ex;
                    }
                };
        }

        [Tags("BL")]
        [Subject(typeof(ProcessOrderCreationRequestSingleOperation))]
        private class When_process_method_called : BasicContext
        {
            It should_create_one_new_order = () =>
                    Mock.Get(ModifyOrderService).Verify(x => x.Modify(Moq.It.IsAny<OrderDomainEntityDto>()), Times.Once);

            It should_request_correct_request = () =>
                    Mock.Get(OrderProcessingRequestService).Verify(x => x.GetProlongationRequestToProcess(Moq.It.Is<long>(id => id == OrderCreationRequestId)));

            It should_request_exactly_one_request = () =>
                    Mock.Get(OrderProcessingRequestService).Verify(x => x.GetProlongationRequestToProcess(Moq.It.IsAny<long>()), Times.Once());
        }

        [Tags("BL")]
        [Subject(typeof(ProcessOrderCreationRequestSingleOperation))]
        private class When_system_exception_thrown : BasicContext
        {
            private Establish context = () =>
                {
                    // На самом деле не важно, который из методов кинет ошибку, важно лишь, что это не BusinessLogicException
                    Mock.Get(OrderProcessingRequestService)
                        .Setup(x => x.GetFirmDto(Moq.It.IsAny<long>()))
                        .Throws(new Exception());
                };

            It should_throw_exception = () =>
                    ExceptionThrown.Should().NotBeNull();

            private It should_write_messages = () =>
                    Mock.Get(OrderProcessingRequestService)
                        .Verify(x => x.SaveMessagesForOrderProcessingRequest(Moq.It.IsAny<long>(), Moq.It.IsAny<IEnumerable<IMessageWithType>>()), Times.AtLeastOnce());
        }

        [Tags("BL")]
        [Subject(typeof(ProcessOrderCreationRequestSingleOperation))]
        private class When_business_logic_exception_thrown : BasicContext
        {
            private static readonly BusinessLogicException ExceptionToThrow = new BusinessLogicException("Some Random Letters");
            private Establish context = () =>
            {
                // На самом деле не важно, который из методов кинет ошибку, важно лишь, что это BusinessLogicException
                Mock.Get(OrderProcessingRequestService)
                    .Setup(x => x.GetFirmDto(Moq.It.IsAny<long>()))
                    .Throws(ExceptionToThrow);
            };

            It should_throw_exception = () =>
                    ExceptionThrown.Should().NotBeNull();

            private It should_write_messages = () =>
                    Mock.Get(OrderProcessingRequestService)
                        .Verify(x => x.SaveMessagesForOrderProcessingRequest(Moq.It.IsAny<long>(), Moq.It.IsAny<IEnumerable<IMessageWithType>>()), Times.AtLeastOnce());
        }
    }
}
// ReSharper restore MemberCanBePrivate.Local
// ReSharper restore ConvertToLambdaExpression