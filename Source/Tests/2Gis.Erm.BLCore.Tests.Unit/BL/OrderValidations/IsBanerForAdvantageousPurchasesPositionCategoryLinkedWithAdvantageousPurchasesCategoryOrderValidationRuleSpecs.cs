﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;
using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderValidations
{
    public class IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRuleSpecs
    {
        abstract class OrderValidationRuleDataContext
        {
            Establish context = () =>
            {
                ValidateOrdersRequest = new ValidateOrdersRequest
                    {
                        OrganizationUnitId = 1,
                        OrderId = 1
                    };

                Firms = new List<Firm>
                        {
                            new Firm
                                {
                                    OrganizationUnitId = 1,
                                    IsActive = true,
                                    IsDeleted = false,
                                    ClosedForAscertainment = false,
                                    FirmAddresses =
                                        {
                                            new FirmAddress
                                                {
                                                    IsActive = true,
                                                    IsDeleted = false,
                                                    CategoryFirmAddresses =
                                                        {
                                                            new CategoryFirmAddress
                                                                {
                                                                    IsActive = true,
                                                                    IsDeleted = false,
                                                                    Category = new Category()
                                                                }
                                                        }
                                                }
                                        }
                                }
                        };

                var position = new Position
                {
                    IsActive = true,
                    IsDeleted = false,
                    Name = "Test",
                    PositionCategory = new PositionCategory(),
                    Platform = new Platform.Model.Entities.Erm.Platform()
                };

                Orders = new List<Order>
                        {
                            new Order
                                {
                                    Id = 1,
                                    DestOrganizationUnitId = 1,
                                    Firm = Firms.Single(),
                                    IsActive = true,
                                    IsDeleted = false,
                                    OrderPositions =
                                        {
                                            new OrderPosition
                                                {
                                                    IsActive = true,
                                                    IsDeleted = false,
                                                    PricePosition = new PricePosition
                                                        {
                                                            IsActive = true,
                                                            IsDeleted = false,
                                                            Position = position
                                                        },
                                                    OrderPositionAdvertisements =
                                                        {
                                                            new OrderPositionAdvertisement
                                                                {
                                                                    Position = position,
                                                                    Category = new Category()
                                                                }
                                                        }
                                                }
                                        }
                                }
                        };

                Orders.Single().OrderPositions.Single().Order = Orders.Single();
            };

            protected static IList<Order> Orders { get; private set; }
            protected static IList<Firm> Firms { get; private set; }
            protected static ValidateOrdersRequest ValidateOrdersRequest { get; private set; }
        }

        abstract class FinderMockContext : OrderValidationRuleDataContext
        {
            static IOrderValidationRule _orderValidationRule;

            Establish context = () =>
                {
                    var finderMock = new Mock<IFinder>();

                    finderMock.Setup(ld => ld.Find(Moq.It.IsAny<Expression<Func<Order, bool>>>()))
                              .Returns((Expression<Func<Order, bool>> predicate) => Orders.Where(predicate.Compile()).AsQueryable());
                    
                    finderMock.Setup(ld => ld.Find(Moq.It.IsAny<IFindSpecification<Order>>()))
                              .Returns((IFindSpecification<Order> predicate) => Orders.Where(predicate.Predicate.Compile()).AsQueryable());

                    _orderValidationRule = new IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule(finderMock.Object);
                };

            Because of = () => Messages = _orderValidationRule.Validate(ValidateOrdersRequest, new OrderValidationPredicate(x => true, null, null));

            protected static IReadOnlyList<OrderValidationMessage> Messages { get; private set; }
        }

        [Tags("BL")]
        [Subject(typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule))]
        class When_validating_with_default_values_as_arguments_by_manual_report_mode : FinderMockContext
        {
            Establish context = () => ValidateOrdersRequest.Type = ValidationType.ManualReport;

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule))]
        class When_validating_with_default_values_as_arguments_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () => ValidateOrdersRequest.Type = ValidationType.SingleOrderOnRegistration;

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule))]
        class When_validating_orders_with_baner_for_advantageous_purchases_position_by_manual_report_mode : FinderMockContext
        {
            Establish context = () =>
                {
                    ValidateOrdersRequest.Type = ValidationType.ManualReport;

                    var orderPositionAdvertisement = Orders.Single().OrderPositions.Single().OrderPositionAdvertisements.Single();
                    
                    orderPositionAdvertisement.CategoryId = IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule.AdvantageousPurchasesCategoryId;
                    orderPositionAdvertisement.Position.CategoryId = IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule.BanerForAdvantageousPurchasesPositionCategoryId;
                };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule))]
        class When_validating_orders_with_baner_for_advantageous_purchases_position_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidateOrdersRequest.Type = ValidationType.SingleOrderOnRegistration;

                var orderPositionAdvertisement = Orders.Single().OrderPositions.Single().OrderPositionAdvertisements.Single();

                orderPositionAdvertisement.CategoryId = IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule.AdvantageousPurchasesCategoryId;
                orderPositionAdvertisement.Position.CategoryId = IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule.BanerForAdvantageousPurchasesPositionCategoryId;
            };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule))]
        class When_validating_orders_with_baner_for_advantageous_purchases_position_linked_to_other_category_by_manual_report_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidateOrdersRequest.Type = ValidationType.ManualReport;

                var orderPositionAdvertisement = Orders.Single().OrderPositions.Single().OrderPositionAdvertisements.Single();

                orderPositionAdvertisement.Position.CategoryId = IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule.BanerForAdvantageousPurchasesPositionCategoryId;
            };

            It should_be_only_message = () => Messages.Should().HaveCount(1);
            It message_text_should_be_equal_to_text_from_IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryError_resource_key =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryError,
                                                                              "Test"));

            It message_should_be_with_error_type = () => Messages.Single().Type.Should().Be(MessageType.Error);
        }

        [Tags("BL")]
        [Subject(typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule))]
        class When_validating_orders_with_baner_for_advantageous_purchases_linked_to_other_category_position_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidateOrdersRequest.Type = ValidationType.SingleOrderOnRegistration;

                var orderPositionAdvertisement = Orders.Single().OrderPositions.Single().OrderPositionAdvertisements.Single();

                orderPositionAdvertisement.Position.CategoryId = IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule.BanerForAdvantageousPurchasesPositionCategoryId;
            };

            It should_be_only_message = () => Messages.Should().HaveCount(1);
            It message_text_should_be_equal_to_text_from_IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryError_resource_key =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryError,
                                                                              "<OrderPosition:Test:0>"));

            It message_should_be_with_error_type = () => Messages.Single().Type.Should().Be(MessageType.Error);
        }
    }
}