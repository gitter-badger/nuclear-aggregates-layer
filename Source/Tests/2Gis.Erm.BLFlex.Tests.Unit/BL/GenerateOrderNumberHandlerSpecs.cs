using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Orders;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BL.Handlers.Orders.Number;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Unit.Tests.BL
{
    public class GenerateOrderNumberHandlerSpecs
    {
        private const string NewOrderNumberFormat = "БЗ_32-01-35537-API";
        private const string OldOrderNumberFormat = "БЗ_50-50-35537";

        [Tags("BL")]
        [Subject(typeof(GenerateOrderNumberHandler))]
        public abstract class OrderRepositoryMockContext
        {
            Establish context = () =>
            {
                var orderRepositoryMock = new Mock<IOrderRepository>();
                orderRepositoryMock.Setup(x => x.GetOrderOrganizationUnitsSyncCodes(Moq.It.IsAny<long[]>()))
                                   .Returns((long[] orderIds) => orderIds.ToDictionary(i => i, i => i.ToString()));

                Handler = new GenerateOrderNumberHandler(orderRepositoryMock.Object);
            };

            protected static GenerateOrderNumberHandler Handler { get; private set; }
        }

        class When_call_Handle_for_old_order_number_format_without_reserved_number_and_not_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    Number = OldOrderNumberFormat,
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    ReservedNumber = null,
                    IsRegionalNumber = false
                });

            It order_number_should_be_with_BZ_prefix_and_with_predefined_order_number = () => _generateOrderNumberResponse.Number.Should().Be("БЗ_666-999-35537");
        }

        class When_call_Handle_for_old_order_number_format_without_reserved_number_and_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    Number = OldOrderNumberFormat,
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    ReservedNumber = null,
                    IsRegionalNumber = true
                });

            It order_number_should_be_with_OF_prefix_and_with_predefined_order_number = () => _generateOrderNumberResponse.Number.Should().Be("ОФ_666-999-35537");
        }

        class When_call_Handle_for_new_order_number_format_without_reserved_number_and_not_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    Number = NewOrderNumberFormat,
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    ReservedNumber = null,
                    IsRegionalNumber = false
                });

            It order_number_should_be_with_BZ_prefix_and_with_predefined_order_number = () => _generateOrderNumberResponse.Number.Should().Be("БЗ_666-999-35537");
        }

        class When_call_Handle_for_new_order_number_format_without_reserved_number_and_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    Number = NewOrderNumberFormat,
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    ReservedNumber = null,
                    IsRegionalNumber = true
                });

            It order_number_should_be_with_OF_prefix_and_with_predefined_order_number = () => _generateOrderNumberResponse.Number.Should().Be("ОФ_666-999-35537");
        }

        class When_call_Handle_without_order_number_format_with_reserved_number_and_not_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    ReservedNumber = 35537,
                    IsRegionalNumber = false
                });

            It order_number_should_be_with_BZ_prefix_and_with_reserved_order_number = () => _generateOrderNumberResponse.Number.Should().Be("БЗ_666-999-35537");
        }

        class When_call_Handle_without_order_number_format_with_reserved_number_and_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    ReservedNumber = 35537,
                    IsRegionalNumber = true
                });

            It order_number_should_be_with_OF_prefix_and_with_reserved_order_number = () => _generateOrderNumberResponse.Number.Should().Be("ОФ_666-999-35537");
        }

        class When_call_Handle_with_order_number_in_fluent_format_not_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    Number = "123456",
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    IsRegionalNumber = false
                });

            It order_number_should_not_be_changed = () => _generateOrderNumberResponse.Number.Should().Be("123456");
        }

        class When_call_Handle_with_order_number_in_fluent_format_for_regional_order : OrderRepositoryMockContext
        {
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    Number = "123456",
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                {
                    Order = _order,
                    IsRegionalNumber = true
                });

            It order_number_should_not_be_changed = () => _generateOrderNumberResponse.Number.Should().Be("123456");
        }

        class When_call_Handle_without_predefined_order_number_and_without_reserved_number_for_not_regional_order : OrderRepositoryMockContext
        {
            static Exception _exception;
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _exception = Catch.Exception(
                () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                    {
                        Order = _order,
                        IsRegionalNumber = false
                    }));

            It exception_of_type_NotificationException_should_be_thrown = () => _exception.Should().BeOfType<NotificationException>();
        }

        class When_call_Handle_without_predefined_order_number_and_without_reserved_number_for_regional_order : OrderRepositoryMockContext
        {
            static Exception _exception;
            static Order _order;
            static GenerateOrderNumberResponse _generateOrderNumberResponse;

            Establish context = () => _order = new Order
                {
                    DestOrganizationUnitId = 999,
                    SourceOrganizationUnitId = 666
                };

            Because of = () => _exception = Catch.Exception(
                () => _generateOrderNumberResponse = (GenerateOrderNumberResponse)Handler.Handle(new GenerateOrderNumberRequest
                    {
                        Order = _order,
                        IsRegionalNumber = false
                    }));

            It exception_of_type_NotificationException_should_be_thrown = () => _exception.Should().BeOfType<NotificationException>();
        }
    }
}