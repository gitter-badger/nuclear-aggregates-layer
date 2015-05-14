using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;
using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderValidations
{
    public class AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRuleSpecs
    {
        abstract class OrderValidationRuleDataContext
        {
            Establish context = () =>
                {
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
                                                                }
                                                        }
                                                }
                                        }
                                }
                        };

                    Orders.Single().OrderPositions.Single().Order = Orders.Single();
                };

            protected const long TargetOrganizationUnitId = 1;
            protected const long TargetOrderId = 1;

            protected static ValidationParams ValidationParams { get; set; }
            protected static IList<Order> Orders { get; private set; }
            protected static IList<Firm> Firms { get; private set; }
        }

        abstract class FinderMockContext : OrderValidationRuleDataContext
        {
            static IOrderValidationRule _orderValidationRule;

            Establish context = () =>
                {
                    var finderMock = new Mock<IFinder>();

                    finderMock.Setup(ld => ld.Find(Moq.It.IsAny<Expression<Func<Order, bool>>>()))
                          .Returns((Expression<Func<Order, bool>> predicate) => Orders.Where(predicate.Compile()).AsQueryable());

                    finderMock.Setup(f => f.Find(Moq.It.IsAny<Expression<Func<Firm, bool>>>()))
                          .Returns((Expression<Func<Firm, bool>> filter) => Firms.Where(filter.Compile()).AsQueryable());

                    finderMock.Setup(ld => ld.Find(Moq.It.IsAny<FindSpecification<Order>>()))
                          .Returns((FindSpecification<Order> predicate) => Orders.Where(predicate.Predicate.Compile()).AsQueryable());

                    _orderValidationRule = new AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule(finderMock.Object);
                };

            Because of = () => Messages = _orderValidationRule.Validate(ValidationParams, new OrderValidationPredicate(x => true, null, null), null);

            protected static IEnumerable<OrderValidationMessage> Messages { get; private set; }
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_with_default_values_as_arguments_by_manual_report_mode : FinderMockContext
        {
            Establish context = () => ValidationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.ManualReport) { OrganizationUnitId = TargetOrganizationUnitId };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_with_self_promotion_for_the_PC_position_category_by_manual_report_mode : FinderMockContext
        {
            Establish context = () =>
                {
                    ValidationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.ManualReport) { OrganizationUnitId = TargetOrganizationUnitId };

                    var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;
                    var positionCategory = Orders.Single().OrderPositions.Single().PricePosition.Position.PositionCategory;

                    // Выгодные покупки с 2ГИС
                    firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
                    positionCategory.ExportCode = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.SelfPromotionForThePcPositionCategoryExportCode;
                };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_with_self_promotion_for_the_PC_position_category_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new SingleOrderValidationParams(Guid.NewGuid(), ValidationType.SingleOrderOnRegistration) { OrderId = TargetOrderId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;
                var positionCategory = Orders.Single().OrderPositions.Single().PricePosition.Position.PositionCategory;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
                positionCategory.ExportCode = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.SelfPromotionForThePcPositionCategoryExportCode;
            };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_with_advantageous_purchases_position_category_for_PC_platform_by_manual_report_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.ManualReport) { OrganizationUnitId = TargetOrganizationUnitId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;
                var positionCategory = Orders.Single().OrderPositions.Single().PricePosition.Position.PositionCategory;
                var positionPlatform = Orders.Single().OrderPositions.Single().PricePosition.Position.Platform;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
                positionCategory.ExportCode = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesPositionCategoryExportCode;
                positionPlatform.DgppId = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.PcPlatform;
            };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_with_advantageous_purchases_position_category_for_PC_platform_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new SingleOrderValidationParams(Guid.NewGuid(), ValidationType.SingleOrderOnRegistration) { OrderId = TargetOrderId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;
                var positionCategory = Orders.Single().OrderPositions.Single().PricePosition.Position.PositionCategory;
                var positionPlatform = Orders.Single().OrderPositions.Single().PricePosition.Position.Platform;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
                positionCategory.ExportCode = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesPositionCategoryExportCode;
                positionPlatform.DgppId = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.PcPlatform;
            };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_with_advantageous_purchases_position_category_for_mobile_platform_by_manual_report_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.ManualReport) { OrganizationUnitId = TargetOrganizationUnitId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;
                var positionCategory = Orders.Single().OrderPositions.Single().PricePosition.Position.PositionCategory;
                var positionPlatform = Orders.Single().OrderPositions.Single().PricePosition.Position.Platform;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
                positionCategory.ExportCode = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesPositionCategoryExportCode;
                positionPlatform.DgppId = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.MobilePlatform;
            };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_with_advantageous_purchases_position_category_for_mobile_platform_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new SingleOrderValidationParams(Guid.NewGuid(), ValidationType.SingleOrderOnRegistration) { OrderId = TargetOrderId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;
                var positionCategory = Orders.Single().OrderPositions.Single().PricePosition.Position.PositionCategory;
                var positionPlatform = Orders.Single().OrderPositions.Single().PricePosition.Position.Platform;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
                positionCategory.ExportCode = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesPositionCategoryExportCode;
                positionPlatform.DgppId = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.MobilePlatform;
            };

            It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_without_advantageous_purchases_position_category_by_manual_report_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.ManualReport) { OrganizationUnitId = TargetOrganizationUnitId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
            };

            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_text_from_ThereIsNoAdvertisementForAdvantageousPurchasesCategory_resource_key =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.ThereIsNoAdvertisementForAdvantageousPurchasesCategory, string.Empty));
            
            It message_should_be_with_error_type = () => Messages.Single().Type.Should().Be(MessageType.Error);
        }

        [Tags("BL")]
        [Subject(typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule))]
        class When_validating_orders_without_advantageous_purchases_position_category_by_single_order_on_registration_mode : FinderMockContext
        {
            Establish context = () =>
            {
                ValidationParams = new SingleOrderValidationParams(Guid.NewGuid(), ValidationType.SingleOrderOnRegistration) { OrderId = TargetOrderId };

                var firmAddressCategory = Firms.Single().FirmAddresses.Single().CategoryFirmAddresses.Single().Category;

                // Выгодные покупки с 2ГИС
                firmAddressCategory.Id = AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule.AdvantageousPurchasesCategoryId;
            };

            It should_be_only_message = () => Messages.Should().HaveCount(1);

            It message_text_should_be_equal_to_text_from_ThereIsNoAdvertisementForAdvantageousPurchasesCategory_resource_key =
                () => Messages.Single().MessageText.Should().Be(string.Format(BLResources.ThereIsNoAdvertisementForAdvantageousPurchasesCategory, "<Firm::0>"));

            It message_should_be_with_warning_type = () => Messages.Single().Type.Should().Be(MessageType.Warning);
        }
    }
}