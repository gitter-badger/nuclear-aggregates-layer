using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.CostCalculationService
{
    public class OrderPositionCostCalculationOperationServiceSpecs
    {
        [Tags("BL")]
        [Subject(typeof(OrderPositionCostCalculationOperationService))]
        public abstract class MockContext
        {
            protected const int ProjectWithOrganizationUnitId = 1;
            protected const int ProjectWithoutOrganizationUnitId = 2;
            protected const decimal DefaultVatRate = 18M;
            protected const long FakePositionId = 1;
            protected const long FakePriceId = 1;
            protected const long FakePricePositionId = 1;

            private static Project[] projects;

            private Establish context = () =>
                {
                    FakePricePosition = new PricePosition
                        {
                            Cost = 50,
                            Id = FakePricePositionId,
                            PriceId = FakePriceId,
                            PositionId = FakePositionId,
                            IsActive = true,
                            IsDeleted = false,
                            Position = new Position
                                {
                                    IsComposite = false
                                }
                        };

                    ProjectWithOrganizationUnit = new Project
                        {
                            OrganizationUnitId = 1,
                            DisplayName = "ProjectWithOrganizationUnit",
                            Id = ProjectWithOrganizationUnitId
                        };

                    ProjectWithoutOrganizationUnit = new Project
                        {
                            OrganizationUnitId = null,
                            DisplayName = "ProjectWithoutOrganizationUnit",
                            Id = ProjectWithoutOrganizationUnitId,
                        };

                    projects = new[]
                        {
                            ProjectWithOrganizationUnit,
                            ProjectWithoutOrganizationUnit
                        };

                    FakeFirm = new Firm
                        {
                            Id = 1,
                        };

                    FinderMock = new Mock<IFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<PricePosition>>()))
                              .Returns(new QueryableSequence<PricePosition>(new[] { FakePricePosition }.AsQueryable()));

                    ProjectServiceMock = new Mock<IProjectService>();
                    ProjectServiceMock.Setup(x => x.GetProjectByCode(Moq.It.IsAny<long>()))
                                      .Returns((long x) => projects.Single(y => y.Id == x));

                    FirmRepositoryMock = new Mock<IFirmRepository>();
                    FirmRepositoryMock.Setup(x => x.GetFirm(Moq.It.IsAny<long>())).Returns(FakeFirm);

                    CalculateOrderCostService = new OrderPositionCostCalculationOperationService(
                        FinderMock.Object,
                        Mock.Of<ICostCalculator>(),
                        Mock.Of<IClientProxyFactory>(),
                        Mock.Of<IOrderReadModel>(),
                        FirmRepositoryMock.Object,
                        Mock.Of<IOperationScopeFactory>(),
                        ProjectServiceMock.Object,
                        Mock.Of<ICalculateCategoryRateOperationService>());
                };

            protected static Mock<IFinder> FinderMock { get; private set; }
            protected static Mock<IProjectService> ProjectServiceMock { get; private set; }
            protected static Mock<IFirmRepository> FirmRepositoryMock { get; private set; }
            protected static DiscountInfo Discount { get; set; }

            protected static CalculationResult CalculationResult { get; set; }

            protected static Project ProjectWithOrganizationUnit
            {
                get;
                set;
            }

            protected static Project ProjectWithoutOrganizationUnit
            {
                get;
                set;
            }

            protected static Firm FakeFirm { get; set; }

            protected static ICalculateOrderPositionCostService CalculateOrderCostService { get; set; }
            private static PricePosition FakePricePosition { get; set; }
        }

        private class When_calculating_order_position_for_self_advertisement_order : MockContext
        {
            private const int defaultReleaseCount = 4;
            private const int amount = 1;
            private const decimal discountSum = 100;
            private const decimal discountPercent = 5;

            private Because of = () =>
                {
                    CalculationResult = CalculateOrderCostService
                        .CalculateOrderPositionCost(OrderType.SelfAds,
                                                    defaultReleaseCount,
                                                    null,
                                                    null,
                                                    FakePositionId,
                                                    FakePriceId,
                                                    amount,
                                                    DefaultVatRate,
                                                    true,
                                                    discountSum,
                                                    discountPercent,
                                                    false);
                };

            private It should_have_zero_discount_sum = () => CalculationResult.DiscountSum.Should().Be(decimal.Zero);
            private It should_have_zero_discount_percent = () => CalculationResult.DiscountPercent.Should().Be(decimal.Zero);
            private It should_have_zero_payable_plan = () => CalculationResult.PayablePlan.Should().Be(decimal.Zero);
            private It should_have_zero_vat = () => CalculationResult.Vat.Should().Be(decimal.Zero);
        }

        private class When_calculating_order_position_for_social_advertisement_order : MockContext
        {
            private const int defaultReleaseCount = 4;
            private const int amount = 1;
            private const decimal discountSum = 100;
            private const decimal discountPercent = 5;

            private Because of = () =>
                {
                    CalculationResult = CalculateOrderCostService
                        .CalculateOrderPositionCost(OrderType.SocialAds,
                                                    defaultReleaseCount,
                                                    null,
                                                    null,
                                                    FakePositionId,
                                                    FakePriceId,
                                                    amount,
                                                    DefaultVatRate,
                                                    true,
                                                    discountSum,
                                                    discountPercent,
                                                    false);
                };

            private It should_have_zero_discount_sum = () => CalculationResult.DiscountSum.Should().Be(decimal.Zero);
            private It should_have_zero_discount_percent = () => CalculationResult.DiscountPercent.Should().Be(decimal.Zero);
            private It should_have_zero_payable_plan = () => CalculationResult.PayablePlan.Should().Be(decimal.Zero);
            private It should_have_zero_vat = () => CalculationResult.Vat.Should().Be(decimal.Zero);
        }

        private class When_calculating_order_positions_with_shortcut_data_and_source_project_has_no_organization_unit : MockContext
        {
            private const int defaultReleaseCount = 4;
            private static readonly DateTime beginDistributionDate = DateTime.Today.GetNextMonthFirstDate();
            private static Exception exception;

            private Because of = () =>
                {
                    exception = Catch.Exception(() => CalculateOrderCostService
                                                          .CalculateOrderPositionsCostWithActualPrice(OrderType.Sale,
                                                                                                      defaultReleaseCount,
                                                                                                      beginDistributionDate,
                                                                                                      ProjectWithoutOrganizationUnitId,
                                                                                                      ProjectWithOrganizationUnitId,
                                                                                                      null,
                                                                                                      null));
                };

            private It invalid_operation_exception_should_be_thrown = () => exception.Should().BeOfType<InvalidOperationException>();

            private It exception_message_should_tell_that_project_has_no_organization_unit =
                () =>
                exception.Message.Should()
                         .Be(string.Format(BLResources.ProjectHasNoOrganizationUnit, ProjectWithoutOrganizationUnit.DisplayName));
        }

        private class When_calculating_order_positions_with_shortcut_data_and_dest_project_has_no_organization_unit : MockContext
        {
            private const int defaultReleaseCount = 4;
            private static readonly DateTime beginDistributionDate = DateTime.Today.GetNextMonthFirstDate();
            private static Exception exception;

            private Because of = () =>
                {
                    exception = Catch.Exception(() => CalculateOrderCostService
                                                          .CalculateOrderPositionsCostWithActualPrice(OrderType.Sale,
                                                                                                      defaultReleaseCount,
                                                                                                      beginDistributionDate,
                                                                                                      ProjectWithOrganizationUnitId,
                                                                                                      ProjectWithoutOrganizationUnitId,
                                                                                                      null,
                                                                                                      null));
                };

            private It invalid_operation_exception_should_be_thrown = () => exception.Should().BeOfType<InvalidOperationException>();

            private It exception_message_should_tell_that_project_has_no_organization_unit =
                () =>
                exception.Message.Should()
                         .Be(string.Format(BLResources.ProjectHasNoOrganizationUnit, ProjectWithoutOrganizationUnit.DisplayName));
        }

        private class When_calculating_order_positions_with_full_data_and_source_project_has_no_organization_unit : MockContext
        {
            private const int defaultReleaseCount = 4;
            private static readonly DateTime beginDistributionDate = DateTime.Today.GetNextMonthFirstDate();
            private static Exception exception;

            private Because of = () =>
                {
                    exception = Catch.Exception(() => CalculateOrderCostService
                                                          .CalculateOrderPositionsCostWithActualPrice(OrderType.Sale,
                                                                                                      defaultReleaseCount,
                                                                                                      beginDistributionDate,
                                                                                                      ProjectWithoutOrganizationUnitId,
                                                                                                      FakeFirm.Id,
                                                                                                      null,
                                                                                                      null));
                };

            private It invalid_operation_exception_should_be_thrown = () => exception.Should().BeOfType<InvalidOperationException>();

            private It exception_message_should_tell_that_project_has_no_organization_unit =
                () =>
                exception.Message.Should()
                         .Be(string.Format(BLResources.ProjectHasNoOrganizationUnit, ProjectWithoutOrganizationUnit.DisplayName));
        }
    }
}
