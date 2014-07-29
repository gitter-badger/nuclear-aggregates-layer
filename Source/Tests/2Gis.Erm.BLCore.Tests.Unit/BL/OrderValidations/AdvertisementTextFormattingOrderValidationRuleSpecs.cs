using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderValidations
{
    public class AdvertisementTextFormattingOrderValidationRuleSpecs
    {
        [Tags("BL")]
        [Subject(typeof(AdvertisementTextFormattingOrderValidationRule))]
        public abstract class FinderMockContext
        {
           static IOrderValidationRule _orderValidationRule;

           Establish context = () =>
                {
                    FinderMock = new Mock<IFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<Expression<Func<Order, bool>>>()))
                              .Returns(new Order[0].AsQueryable());
                    _orderValidationRule = new AdvertisementTextFormattingOrderValidationRule(FinderMock.Object);
                };

            Because of = () => Messages = _orderValidationRule.Validate(new OrderValidationPredicate(null, null, null), null, new ValidateOrdersRequest());

            protected static Mock<IFinder> FinderMock { get; private set; }
            protected static IReadOnlyList<OrderValidationMessage> Messages { get; private set; }
        }

        public abstract class ValidOrderContext : FinderMockContext
        {
            Establish context = () =>
                {
                    var orderPositionAdvertisement = new OrderPositionAdvertisement
                        {
                            Advertisement = new Advertisement
                                {
                                    AdvertisementElements =
                                        {
                                            new AdvertisementElement
                                                {
                                                    IsDeleted = false,
                                                    AdvertisementElementTemplate = new AdvertisementElementTemplate
                                                        {
                                                            RestrictionType = (int)AdvertisementElementRestrictionType.Text,
                                                            MaxSymbolsInWord = 20
                                                        },
                                                    Text = @"Обычный текст. Здесь нет никаких проблем с форматированием"
                                                }
                                        }
                                }
                        };
                    
                    orderPositionAdvertisement.Advertisement.AdvertisementElements.Single().Advertisement = orderPositionAdvertisement.Advertisement;

                    Order = new Order
                        {
                            OrderPositions =
                                {
                                    new OrderPosition
                                        {
                                            Order = Order,
                                            IsActive = true,
                                            IsDeleted = false,
                                            PricePosition = new PricePosition
                                                {
                                                    Position = new Position { Name = "Test" }
                                                },
                                            OrderPositionAdvertisements = { orderPositionAdvertisement }
                                        }
                                }
                        };

                    Order.OrderPositions.Single().Order = Order;

                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<Expression<Func<Order, bool>>>()))
                              .Returns(new[] { Order }.AsQueryable());
                };

            protected static Order Order { get; private set; }
        }

        public abstract class OrderWithNonBreakingSpaceContext : ValidOrderContext
        {
            Establish context = () => Order.OrderPositions
                                           .Single()
                                           .OrderPositionAdvertisements
                                           .Single()
                                           .Advertisement.AdvertisementElements
                                           .Single()
                                           .Text = "Неразрывный" + ((char)160) + "пробел";
        }

        public abstract class OrderWithRestrictedSymbolsIContext : ValidOrderContext
        {
            Establish context = () => Order.OrderPositions
                                           .Single()
                                           .OrderPositionAdvertisements
                                           .Single()
                                           .Advertisement.AdvertisementElements
                                           .Single()
                                           .Text = @"Этот текст \i содержит запрещенную комбинацию символов";
        }

        public abstract class OrderWithRestrictedSymbolsRContext : ValidOrderContext
        {
            Establish context = () => Order.OrderPositions
                                           .Single()
                                           .OrderPositionAdvertisements
                                           .Single()
                                           .Advertisement.AdvertisementElements
                                           .Single()
                                           .Text = @"Этот текст \r содержит запрещенную комбинацию символов";
        }

        public abstract class OrderWithRestrictedSymbolsNContext : ValidOrderContext
        {
            Establish context = () => Order.OrderPositions
                                           .Single()
                                           .OrderPositionAdvertisements
                                           .Single()
                                           .Advertisement.AdvertisementElements
                                           .Single()
                                           .Text = @"Этот текст \n содержит запрещенную комбинацию символов";
        }

        public abstract class OrderWithRestrictedSymbolsPContext : ValidOrderContext
        {
            Establish context = () => Order.OrderPositions
                                           .Single()
                                           .OrderPositionAdvertisements
                                           .Single()
                                           .Advertisement.AdvertisementElements
                                           .Single()
                                           .Text = @"Этот текст \p содержит запрещенную комбинацию символов";
        }

        public abstract class OrderWithTooManySymbolsInAWord : ValidOrderContext
        {
            Establish context = () =>
                {
                    var advertisementElement = Order.OrderPositions
                                                    .Single()
                                                    .OrderPositionAdvertisements
                                                    .Single()
                                                    .Advertisement.AdvertisementElements
                                                    .Single();

                    advertisementElement.Text = "Этот текст содержит ОченьПреоченьДлинноеСлово";
                    advertisementElement.AdvertisementElementTemplate.MaxSymbolsInWord = 15;
                };
        }

        class When_validating_with_default_values_as_arguments : FinderMockContext
        {
            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        class When_validating_correct_order : ValidOrderContext
        {
            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        class When_validating_order_with_breaking_space_symbol_in_advertisement_element_text : OrderWithNonBreakingSpaceContext
        {
            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_NonBreakingSpaceError_key_from_Resources =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.NonBreakingSpaceError, "<AdvertisementElement::0>"));
        } 
        
        class When_validating_order_with_i_symbol_in_advertisement_element_text : OrderWithRestrictedSymbolsIContext
        {
            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_RestrictedSymbolsInAdvertisementElementText_key_from_Resources =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.RestrictedSymbolsInAdvertisementElementText, "<AdvertisementElement::0>", @"\i"));
        }

        class When_validating_order_with_r_symbol_in_advertisement_element_text : OrderWithRestrictedSymbolsRContext
        {
            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_RestrictedSymbolsInAdvertisementElementText_key_from_Resources =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.RestrictedSymbolsInAdvertisementElementText, "<AdvertisementElement::0>", @"\r"));
        }
        
        class When_validating_order_with_n_symbol_in_advertisement_element_text : OrderWithRestrictedSymbolsNContext
        {
            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_RestrictedSymbolsInAdvertisementElementText_key_from_Resources =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.RestrictedSymbolsInAdvertisementElementText, "<AdvertisementElement::0>", @"\n"));
        }

        class When_validating_order_with_p_symbol_in_advertisement_element_text : OrderWithRestrictedSymbolsPContext
        {
            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_RestrictedSymbolsInAdvertisementElementText_key_from_Resources =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.RestrictedSymbolsInAdvertisementElementText, "<AdvertisementElement::0>", @"\p"));
        }

        [Ignore("Код для проверки длины слова отсутствует в коде проверки AdvertisementTextFormattingOrderValidationRule")]
        class When_validating_order_with_too_many_symbols_in_a_word_in_advertisement_element_text : OrderWithTooManySymbolsInAWord
        {
            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_TooLongWordInAdvertisementElementError_key_from_Resources =
                () => Messages.Single().MessageText.Should().Be(BLResources.TooLongWordInAdvertisementElementError);
        } 
    }
}