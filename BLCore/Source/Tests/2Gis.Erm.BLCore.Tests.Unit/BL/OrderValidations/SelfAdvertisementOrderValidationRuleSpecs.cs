using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderValidations
{
    [Subject(typeof(SelfAdvertisementOrderValidationRule))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
    internal class SelfAdvertisementOrderValidationRuleSpecs
    {
        [Tags("BL")]
        private abstract class SelfAdvertisementOrderValidationRuleContext
        {
            protected static SelfAdvertisementOrderValidationRule Target;
            protected static IFinder Finder;
            protected static MassOrdersValidationParams ValidationParams;
            protected static OrderValidationPredicate Predicate;
            protected static IEnumerable<OrderValidationMessage> ValidationMessages;

            private Establish context = () =>
                {
                    Finder = SetupFinder();

                    ValidationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.PreReleaseBeta);
                    Predicate = new OrderValidationPredicate(order => true, order => true, order => true);

                    Target = new SelfAdvertisementOrderValidationRule(Mock.Of<IQuery>());
                }; 
            
            private Because of = () => ValidationMessages = ((IOrderValidationRule)Target).Validate(ValidationParams, Predicate, null);

            private static IFinder SetupFinder()
                {
                    var finder = Mock.Of<IFinder>();

                    return finder;
                }
        }

        private class SingleOrderContext : SelfAdvertisementOrderValidationRuleContext
        {
            protected const long ORDER_ID = 111111111111;
            protected const long FIRM_ID = 22222222222;
            protected const string ORDER_NUMBER = "TEST_ORDER_NUMBER";

            // Самореклама только для ПК
            protected const int SelfAdvertisementPositionCategoryId = 287;
            protected static Order Order;

            private Establish context = () =>
            {
                Order = new Order
                {
                    Id = ORDER_ID,
                    Number = ORDER_NUMBER,
                    FirmId = FIRM_ID
                };

                Mock.Get(Finder)
                    .Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Order>>()))
                    .Returns(new QueryableFutureSequence<Order>(new[] { Order }.AsQueryable()));
            };

            protected static OrderPosition CreateSelfAdvertisementPosition()
            {
                return CreateOrderPosition(SelfAdvertisementPositionCategoryId, PlatformEnum.Desktop);
            }

            protected static OrderPosition CreateDesktopPosition()
            {
                return CreateOrderPosition(0, PlatformEnum.Desktop);
            }

            protected static OrderPosition CreateNotDesktopPosition()
            {
                return CreateOrderPosition(0, (PlatformEnum)1488); // точно не Platform.Desktop и не Platform.Independent
            }

            protected static OrderPosition CreateOrderPosition(long positionCategoryId, PlatformEnum platformType)
            {
                var platform = new Platform.Model.Entities.Erm.Platform { DgppId = (long)platformType };

                var position = new Position
                {
                    CategoryId = positionCategoryId,
                    Platform = platform,
                    IsActive = true,
                    IsDeleted = false
                };

                var orderPosition = new OrderPosition
                {
                    OrderPositionAdvertisements = new[] { new OrderPositionAdvertisement { Position = position } },
                    IsActive = true,
                    IsDeleted = false
                };

                return orderPosition;
            }
        }

        private class DoubleOrdersContext : SingleOrderContext
        {
            protected const long ORDER_ID_2 = ORDER_ID + 1;
            protected const long FIRM_ID_2 = FIRM_ID + 1;
            protected const string ORDER_NUMBER_2 = ORDER_NUMBER + "_2";

            protected static Order Order_2;

            private Establish context = () =>
            {
                Order_2 = new Order
                {
                    Id = ORDER_ID_2,
                    Number = ORDER_NUMBER_2,
                    FirmId = FIRM_ID_2
                };

                Mock.Get(Finder)
                    .Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Order>>()))
                    .Returns(new QueryableFutureSequence<Order>(new[] { Order, Order_2 }.AsQueryable()));
            };
        }

        private class TripleOrdersContext : DoubleOrdersContext
        {
            protected const long ORDER_ID_3 = ORDER_ID_2 + 1;
            protected const long FIRM_ID_3 = FIRM_ID_2 + 1;
            protected const string ORDER_NUMBER_3 = ORDER_NUMBER + "_3";

            protected static Order Order_3;

            private Establish context = () =>
            {
                Order_3 = new Order
                {
                    Id = ORDER_ID_3,
                    Number = ORDER_NUMBER_3,
                    FirmId = FIRM_ID_3
                };

                Mock.Get(Finder)
                    .Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Order>>()))
                    .Returns(new QueryableFutureSequence<Order>(new[] { Order, Order_2, Order_3 }.AsQueryable()));
            };
        }

        private class When_there_is_no_order : SelfAdvertisementOrderValidationRuleContext
        {
            private It should_returns_empty_ValidationMessages_collection = () => ValidationMessages.Should().BeEmpty();
        }

        private class When_there_is_correct_order_only : SingleOrderContext
        {
            private Establish context = () =>
            {
                Order.OrderPositions = new[]
                        {
                            CreateSelfAdvertisementPosition(),
                            CreateDesktopPosition()
                        };
            };

            private It should_returns_empty_ValidationMessages_collection = () => ValidationMessages.Should().BeEmpty();
        }

        private class When_there_is_incorrect_order_only : SingleOrderContext
        {
            private Establish context = () =>
            {
                Order.OrderPositions = new[]
                        {
                            CreateSelfAdvertisementPosition(),
                            CreateNotDesktopPosition()
                        };
            };

            private It should_returns_single_validation_message = () => ValidationMessages.Count().Should().Be(1);

            private It should_returns_validation_message_about_order =
                () => ValidationMessages.Should().Contain(x => x.OrderId == ORDER_ID && x.OrderNumber == ORDER_NUMBER);
        }

        private class When_there_are_two_incorrect_orders : DoubleOrdersContext
        {
            private Establish context = () =>
            {
                Order.OrderPositions = new[]
                        {
                            CreateSelfAdvertisementPosition(),
                            CreateNotDesktopPosition()
                        };

                Order_2.OrderPositions = new[]
                        {
                            CreateSelfAdvertisementPosition(),
                            CreateNotDesktopPosition()
                        };
            };

            private It should_returns_double_validation_message = () => ValidationMessages.Count().Should().Be(2);

            private It should_returns_validation_message_about_first_order =
                () => ValidationMessages.Should().Contain(x => x.OrderId == ORDER_ID && x.OrderNumber == ORDER_NUMBER);

            private It should_returns_validation_message_about_second_order =
                () => ValidationMessages.Should().Contain(x => x.OrderId == ORDER_ID_2 && x.OrderNumber == ORDER_NUMBER_2);
        }

        private class When_denied_positions_are_in_different_orders : DoubleOrdersContext
        {
            private Establish context = () =>
                {
                    Order.FirmId = FIRM_ID;
                    Order_2.FirmId = FIRM_ID;

                    Order.OrderPositions = new[] { CreateNotDesktopPosition() };
                    Order_2.OrderPositions = new[] { CreateSelfAdvertisementPosition() }; 
                };

            private It should_returns_single_validation_message = () => ValidationMessages.Count().Should().Be(1);

            private It should_returns_validation_message_about_selfAdv_order =
                () => ValidationMessages.Should().Contain(x => x.OrderId == ORDER_ID_2 && x.OrderNumber == ORDER_NUMBER_2);
        }

        private class When_denied_positions_are_in_different_triple_orders : TripleOrdersContext
        {
            private Establish context = () =>
            {
                Order.FirmId = FIRM_ID;
                Order_2.FirmId = FIRM_ID;
                Order_3.FirmId = FIRM_ID;

                Order.OrderPositions = new[] { CreateNotDesktopPosition() };
                Order_2.OrderPositions = new[] { CreateSelfAdvertisementPosition() };
                Order_3.OrderPositions = new[] { CreateNotDesktopPosition() };
            };

            private It should_returns_single_validation_message = () => ValidationMessages.Count().Should().Be(1);

            private It should_returns_validation_message_about_selfAdv_order =
                () => ValidationMessages.Should().Contain(x => x.OrderId == ORDER_ID_2 && x.OrderNumber == ORDER_NUMBER_2);
        }

        private class When_correct_positions_are_in_different_triple_orders : TripleOrdersContext
        {
            private Establish context = () =>
            {
                Order.FirmId = FIRM_ID;
                Order_2.FirmId = FIRM_ID;
                Order_3.FirmId = FIRM_ID;

                Order.OrderPositions = new[] { CreateDesktopPosition() };
                Order_2.OrderPositions = new[] { CreateSelfAdvertisementPosition() };
                Order_3.OrderPositions = new[] { CreateDesktopPosition() };
            };

            private It should_returns_empty_ValidationMessages_collection = () => ValidationMessages.Should().BeEmpty();
        }

        private class When_there_are_two_selfAdv_orders_and_notDesktop_order : TripleOrdersContext
        {
            private Establish context = () =>
            {
                Order.FirmId = FIRM_ID;
                Order_2.FirmId = FIRM_ID;
                Order_3.FirmId = FIRM_ID;

                Order.OrderPositions = new[] { CreateSelfAdvertisementPosition() };
                Order_2.OrderPositions = new[] { CreateNotDesktopPosition() };
                Order_3.OrderPositions = new[] { CreateSelfAdvertisementPosition() };
            };

            private It should_returns_single_validation_message = () => ValidationMessages.Count().Should().Be(1);

            private It should_returns_validation_message_about_first_selfAdv_order =
                () => ValidationMessages.Should().Contain(x => x.OrderId == ORDER_ID && x.OrderNumber == ORDER_NUMBER);
        }
    }
}
