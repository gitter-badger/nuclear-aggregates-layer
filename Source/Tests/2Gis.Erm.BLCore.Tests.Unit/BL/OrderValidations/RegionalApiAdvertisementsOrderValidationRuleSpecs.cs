using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;
using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderValidations
{
    public class RegionalApiAdvertisementsOrderValidationRuleSpecs
    {
        [Tags("BL")]
        [Tags("OrderValidation")]
        [Subject(typeof(RegionalApiAdvertisementsOrderValidationRule))]
        private abstract class RegionalApiAdvertisementsOrderValidationRuleSpecsContext
        {
            private static IOrderValidationRule orderValidationRule;

            private Establish context = () =>
                {
                    FinderMock = new Mock<IFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<Expression<Func<Order, bool>>>()))
                              .Returns(new Order[0].AsQueryable());
                    orderValidationRule = new RegionalApiAdvertisementsOrderValidationRule(FinderMock.Object);
                };

            private Because of = () =>
                {
                    Messages = orderValidationRule.Validate(
                        new ValidateOrdersRequest { Type = ValidationType.SingleOrderOnRegistration },
                        new OrderValidationPredicate(null, null, null));
                };

            protected static Mock<IFinder> FinderMock { get; private set; }
            protected static IReadOnlyList<OrderValidationMessage> Messages { get; private set; }

            protected static OrganizationUnit GetOrganizationUnit(int index)
            {
                var organizationUnit = new OrganizationUnit
                    {
                        Id = index,
                        IsActive = true,
                        IsDeleted = false,
                        Projects = new List<Project>()
                    };

                var project = GetProject(index);
                project.OrganizationUnit = organizationUnit;
                organizationUnit.Projects.Add(project);

                return organizationUnit;
            }

            protected static FirmAddress GetAddressWithoutBuilding(int index)
            {
                var address = new FirmAddress
                {
                    IsActive = true,
                    IsDeleted = false,
                    ClosedForAscertainment = false,
                    Id = index
                };

                return address;
            }

            protected static FirmAddress GetAddress(int index)
            {
                var address = GetAddressWithoutBuilding(index);

                address.BuildingCode = index;
                address.Building = new Building
                    {
                        Territory = new Territory
                            {
                                OrganizationUnit = GetOrganizationUnit(index)
                            }
                    };

                return address;
            }

            private static Project GetProject(int index)
            {
                var project = new Project
                    {
                        Id = index,
                        IsActive = true,
                    };

                return project;
            }
        }

        // Валидный заказ. Продажа Api есть. Проекты заказа и адреса совпадают.
        private abstract class ValidOrderContext : RegionalApiAdvertisementsOrderValidationRuleSpecsContext
        {
            private Establish context = () =>
                {
                    Order = new Order
                        {
                            Firm = new Firm
                                {
                                    IsActive = true,
                                    IsDeleted = false,
                                    FirmAddresses = new List<FirmAddress>
                                        {
                                            GetAddress(1)
                                        }
                                },
                            DestOrganizationUnit = GetOrganizationUnit(1),
                            OrderPositions = new List<OrderPosition>
                                {
                                    new OrderPosition
                                        {
                                            IsDeleted = false,
                                            IsActive = true,
                                            Id = 1,
                                            PricePosition = new PricePosition
                                                {
                                                    Position = new Position
                                                        {
                                                            Name = "Тестовая позиция",
                                                            AdvertisementTemplateId = 0,
                                                        }
                                                },
                                            OrderPositionAdvertisements = new[]
                                                {
                                                    new OrderPositionAdvertisement
                                                        {
                                                            Position = new Position
                                                                {
                                                                    IsComposite = false,
                                                                    PlatformId = (long)PlatformEnum.Api,
                                                                    AdvertisementTemplateId = 0,
                                                                }
                                                        }
                                                },
                                        }
                                }
                                
                        };

                    Order.OrderPositions.Single().Order = Order;

                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<Expression<Func<Order, bool>>>()))
                              .Returns(new[] { Order }.AsQueryable());
                };

            protected static Order Order { get; private set; }
        }

        // Валидный заказ. Продажа Api есть. У фирмы заказа 2 адреса в одном проекте. Проекты заказа и адресов совпадают.
        private abstract class ValidOrderWithTwoFirmAddressesContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1),
                                    GetAddress(1)
                                };
                    Order.DestOrganizationUnit = GetOrganizationUnit(1);
                };
        }

        // Валидный заказ. Продажа Api есть. У фирмы заказа 2 адреса в разных проектах. Один из проектов фирмы совпадает с проектом заказа
        private abstract class ValidOrderWithTwoFirmAddressesAndDifferentProjectsContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1),
                                    GetAddress(2)
                                };
                    Order.DestOrganizationUnit = GetOrganizationUnit(1);
                };
        }

        // Валидный заказ. Продажи Api нет. Проекты заказа и адреса разные.
        private abstract class ValidOrderWithoutApiContext : ValidOrderContext
        {
            Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1)
                                };
                    Order.DestOrganizationUnit = GetOrganizationUnit(2);

                    Order.OrderPositions
                         .First()
                         .OrderPositionAdvertisements
                         .First()
                         .Position.Platform = new DoubleGis.Erm.Platform.Model.Entities.Erm.Platform
                             {
                                 Id = (long)PlatformEnum.Desktop,
                                 DgppId = (long)PlatformEnum.Desktop
                             };
                };
        }

        // Невалидный заказ. Продажа Api есть. Проекты заказа и адреса разные.
        private abstract class InvalidOrderContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1)
                                };

                    Order.DestOrganizationUnit = GetOrganizationUnit(2);
                };
        }

        // Невалидный заказ. Продажа Api есть. У фирмы заказа 2 адреса в разных проектах. Один из проектов фирмы совпадает с проектом заказа, но он неактивен
        private abstract class InvalidOrderWithTwoFirmAddressesOneAddressIsInactiveProjectsContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1),
                                    GetAddress(2)
                                };
                    Order.DestOrganizationUnit = GetOrganizationUnit(1);
                    Order.Firm.FirmAddresses.Single(x => x.Id == 1).IsActive = false;
                };
        }

        // Невалидный заказ. Продажа Api есть. У фирмы заказа 2 адреса в разных проектах. Один из проектов фирмы совпадает с проектом заказа, но он неактивен
        private abstract class InvalidOrderWithTwoFirmAddressesOneAddressIsDeletedProjectsContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1),
                                    GetAddress(2)
                                };
                    Order.DestOrganizationUnit = GetOrganizationUnit(1);
                    Order.Firm.FirmAddresses.Single(x => x.Id == 1).IsDeleted = true;
                };
        }

        // Невалидный заказ. Продажа Api есть. У фирмы заказа 2 адреса в разных проектах. Один из проектов фирмы совпадает с проектом заказа, но он неактивен
        private abstract class InvalidOrderWithTwoFirmAddressesOneAddressIsClosedForAscertainmentProjectsContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1),
                                    GetAddress(2)
                                };
                    Order.DestOrganizationUnit = GetOrganizationUnit(1);
                    Order.Firm.FirmAddresses.Single(x => x.Id == 1).ClosedForAscertainment = true;
                };
        }

        // Невалидный заказ. Продажа Api есть. У фирмы заказа 2 адреса в разных проектах. Проекты заказа и адреса разные.
        private abstract class InvalidOrderWithTwoFirmAddressesAndDifferentProjectsContext : ValidOrderContext
        {
            private Establish context = () =>
                {
                    Order.Firm.FirmAddresses = new[]
                                {
                                    GetAddress(1),
                                    GetAddress(2)
                                };

                    Order.DestOrganizationUnit = GetOrganizationUnit(3);
                };
        }

        // Невалидный заказ. Продажа Api есть. У фирмы нет адресов
        private abstract class InvalidOrderWithoutAddressesContext : ValidOrderContext
        {
            private Establish context = () => Order.Firm.FirmAddresses.Clear();
        }

        // Невалидный заказ. Продажа Api есть. У фирмы нет адресов, привязанных к зданию
        private abstract class InvalidOrderWithAddressWithoutBuildingContext : ValidOrderContext
        {
            private Establish context = () =>
            {
                Order.Firm.FirmAddresses = new[] { GetAddressWithoutBuilding(1) };

                Order.DestOrganizationUnit = GetOrganizationUnit(2);
            };
        }

        // Невалидный заказ. Продажа Api есть. У фирмы нет адресов. Фирма неактивна
        private abstract class InvalidOrderNoActiveFirmContext : InvalidOrderWithoutAddressesContext
        {
            private Establish context = () => Order.Firm.IsActive = false;
        }

        // Невалидный заказ. Продажа Api есть. У фирмы нет адресов. Фирма удалена
        private abstract class InvalidOrderDeletedFirmContext : InvalidOrderWithoutAddressesContext
        {
            private Establish context = () => Order.Firm.IsDeleted = true;
        }

        private class When_validating_with_default_values_as_arguments : RegionalApiAdvertisementsOrderValidationRuleSpecsContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        private class When_validating_correct_order : ValidOrderContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        private class When_validating_correct_order_with_2_addresses : ValidOrderWithTwoFirmAddressesContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        private class When_validating_correct_order_with_addresses_in_defferent_projects : ValidOrderWithTwoFirmAddressesAndDifferentProjectsContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        private class When_validating_correct_order_without_api_advertisement : ValidOrderWithoutApiContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        private class When_validating_incorrect_order : InvalidOrderContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }

        private class When_validating_incorrect_order_with_2_addresses_and_one_of_them_is_inactive :
            InvalidOrderWithTwoFirmAddressesOneAddressIsInactiveProjectsContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }

        private class When_validating_incorrect_order_with_2_addresses_and_one_of_them_is_deleted :
            InvalidOrderWithTwoFirmAddressesOneAddressIsDeletedProjectsContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }

        private class When_validating_incorrect_order_with_2_addresses_and_one_of_them_is_closed_for_ascertainment :
            InvalidOrderWithTwoFirmAddressesOneAddressIsClosedForAscertainmentProjectsContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }

        private class When_validating_incorrect_order_with_2_addresses : InvalidOrderWithTwoFirmAddressesAndDifferentProjectsContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }

        private class When_validating_incorrect_order_without_addresses : InvalidOrderWithoutAddressesContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }
        
        private class When_validating_incorrect_order_with_address_without_building : InvalidOrderWithAddressWithoutBuildingContext
        {
            private It should_have_one_message = () => Messages.Should().HaveCount(1);

            private It should_have_warning =
                () => Messages.Should().Contain(x => x.Type == MessageType.Warning && x.OrderNumber == Order.Number && x.OrderId == Order.Id);
        }

        private class When_validating_incorrect_order_with_no_active_firm : InvalidOrderNoActiveFirmContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }

        private class When_validating_incorrect_order_with_deleted_firm : InvalidOrderDeletedFirmContext
        {
            private It should_be_no_messages = () => Messages.Should().BeEmpty();
        }
    }
}