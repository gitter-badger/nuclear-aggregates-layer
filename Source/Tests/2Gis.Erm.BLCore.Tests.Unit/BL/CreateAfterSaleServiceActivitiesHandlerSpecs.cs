using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Deals;
using DoubleGis.Erm.BLCore.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.AfterSalesService;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL
{
    public class CreateAfterSaleServiceActivitiesHandlerSpecs
    {
        private static readonly DateTime BeginDistributionDate = new DateTime(2012, 10, 1);
        private static readonly int ReleaseNumber = BeginDistributionDate.ToAbsoluteReleaseNumber();

        static CreateAfterSaleServiceActivitiesHandler _handler;

        abstract class CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () =>
                {
                    DealDtos = new List<CreateAfterSaleServiceActivitiesHandler.DealDto>();
                    ExistingASSes = new Dictionary<long, List<AfterSaleServiceActivity>>();
                    _handler = new CreateAfterSaleServiceActivitiesHandler(Mock.Of<IFinder>(), Mock.Of<IDealCreateAfterSaleServiceAggregateService>());
                };

            Because of = () => Result = _handler.CreateAfterSaleServiceActivities(DealDtos, ExistingASSes);

            protected static List<CreateAfterSaleServiceActivitiesHandler.DealDto> DealDtos { get; private set; }
            protected static Dictionary<long, List<AfterSaleServiceActivity>> ExistingASSes { get; private set; }
            protected static List<AfterSaleServiceActivity> Result { get; private set; }
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_with_empty_args : CreateAfterSaleServiceActivitiesContext
        {
            It result_should_be_empty = () => Result.Should().BeEmpty();
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_for_single_month_order : CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () => DealDtos.Add(new CreateAfterSaleServiceActivitiesHandler.DealDto
                {
                    DealId = 1,
                    Orders = new[]
                        {
                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                {
                                    BeginDistributionDate = BeginDistributionDate,
                                    ReleaseCountFact = 1
                                }
                        }
                });

            It should_be_only_activity_created = () => Result.Should().HaveCount(1);
            It activity_should_be_for_the_defined_month = () => Result.Single().AbsoluteMonthNumber.Should().Be(ReleaseNumber);
            It activity_should_be_for_the_defined_deal = () => Result.Single().DealId.Should().Be(1);
            It activity_should_be_of_ASS4_type = () => ((AfterSaleServiceType)Result.Single().AfterSaleServiceType).Should().Be(AfterSaleServiceType.ASS4);
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_for_two_single_month_orders : CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () => DealDtos.Add(new CreateAfterSaleServiceActivitiesHandler.DealDto
                {
                    DealId = 1,
                    Orders = new[]
                        {
                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                {
                                    BeginDistributionDate = BeginDistributionDate,
                                    ReleaseCountFact = 1
                                },
                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                {
                                    BeginDistributionDate = BeginDistributionDate,
                                    ReleaseCountFact = 1
                                }
                        }
                });

            It should_be_only_activity = () => Result.Should().HaveCount(1);
            It activity_should_be_for_the_defined_month = () => Result.Single().AbsoluteMonthNumber.Should().Be(ReleaseNumber);
            It activity_should_be_for_the_defined_deal = () => Result.Single().DealId.Should().Be(1);
            It activity_should_be_of_ASS4_type = () => ((AfterSaleServiceType)Result.Single().AfterSaleServiceType).Should().Be(AfterSaleServiceType.ASS4);
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_for_three_orders_starting_from_the_same_date : CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () => DealDtos.Add(new CreateAfterSaleServiceActivitiesHandler.DealDto
                {
                    DealId = 1,
                    Orders = new[]
                        {
                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                {
                                    BeginDistributionDate = BeginDistributionDate,
                                    ReleaseCountFact = 1
                                },
                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                {
                                    BeginDistributionDate = BeginDistributionDate,
                                    ReleaseCountFact = 1
                                },
                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                {
                                    BeginDistributionDate = BeginDistributionDate,
                                    ReleaseCountFact = 2
                                }
                        }
                });

            It should_be_three_activities_created = () => Result.Should().HaveCount(3);
            It first_activity_should_be_of_ASS1_type = () => Result.OfReleseNumber(ReleaseNumber).OfAfterSaleServiceType(AfterSaleServiceType.ASS1).Should().HaveCount(1);
            It second_activity_should_be_of_ASS4_type = () => Result.OfReleseNumber(ReleaseNumber).OfAfterSaleServiceType(AfterSaleServiceType.ASS4).Should().HaveCount(1);
            It third_activity_should_be_of_ASS4_type = () => Result.OfReleseNumber(ReleaseNumber + 1).OfAfterSaleServiceType(AfterSaleServiceType.ASS4).Should().HaveCount(1);
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_for_three_orders_starting_from_the_same_date_with_already_existing_ASS4_activity : CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () =>
                {
                    DealDtos.Add(new CreateAfterSaleServiceActivitiesHandler.DealDto
                        {
                            DealId = 1,
                            Orders = new[]
                                {
                                    new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                        {
                                            BeginDistributionDate = BeginDistributionDate,
                                            ReleaseCountFact = 1
                                        },
                                    new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                        {
                                            BeginDistributionDate = BeginDistributionDate,
                                            ReleaseCountFact = 1
                                        },
                                    new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                        {
                                            BeginDistributionDate = BeginDistributionDate,
                                            ReleaseCountFact = 2
                                        }
                                }
                        });
                    ExistingASSes.Add(1,
                                      new List<AfterSaleServiceActivity>
                                          {
                                              new AfterSaleServiceActivity
                                                  {
                                                      AbsoluteMonthNumber = ReleaseNumber,
                                                      AfterSaleServiceType = (byte)AfterSaleServiceType.ASS4,
                                                  }
                                          });
                };

            It should_be_two_activities_in_total = () => Result.Should().HaveCount(2);
            It first_activity_should_be_of_ASS1_type = () => Result.OfReleseNumber(ReleaseNumber).OfAfterSaleServiceType(AfterSaleServiceType.ASS1).Should().HaveCount(1);
            It second_activity_should_be_of_ASS4_type = () => Result.OfReleseNumber(ReleaseNumber + 1).OfAfterSaleServiceType(AfterSaleServiceType.ASS4).Should().HaveCount(1);
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_for_four_month_order_with_already_existing_ASS3_activity : CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () =>
                {
                    DealDtos.Add(new CreateAfterSaleServiceActivitiesHandler.DealDto
                        {
                            DealId = 1,
                            Orders = new[]
                                {
                                    new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                        {
                                            BeginDistributionDate = BeginDistributionDate,
                                            ReleaseCountFact = 4
                                        }
                                }
                        });
                    ExistingASSes.Add(1,
                                      new List<AfterSaleServiceActivity>
                                          {
                                              new AfterSaleServiceActivity
                                                  {
                                                      AbsoluteMonthNumber = ReleaseNumber + 1,
                                                      AfterSaleServiceType = (byte)AfterSaleServiceType.ASS3,
                                                  }
                                          });
                };

            It should_be_three_activities_in_total = () => Result.Should().HaveCount(3);
            It first_activity_should_be_of_ASS1_type = () => Result.OfReleseNumber(ReleaseNumber).OfAfterSaleServiceType(AfterSaleServiceType.ASS1).Should().HaveCount(1);
            It second_activity_should_be_of_ASS3_type = () => Result.OfReleseNumber(ReleaseNumber + 2).OfAfterSaleServiceType(AfterSaleServiceType.ASS3).Should().HaveCount(1);
            It third_activity_should_be_of_ASS4_type = () => Result.OfReleseNumber(ReleaseNumber + 3).OfAfterSaleServiceType(AfterSaleServiceType.ASS4).Should().HaveCount(1);
        }

        [Tags("BL")]
        [Subject(typeof(CreateAfterSaleServiceActivitiesHandler))]
        class When_creating_after_sale_service_activities_for_orders_in_multiple_deals : CreateAfterSaleServiceActivitiesContext
        {
            Establish context = () =>
                {
                    DealDtos.AddRange(new[]
                        {
                            new CreateAfterSaleServiceActivitiesHandler.DealDto
                                {
                                    DealId = 1,
                                    Orders = new[]
                                        {
                                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                                {
                                                    BeginDistributionDate = BeginDistributionDate,
                                                    ReleaseCountFact = 3
                                                }
                                        }
                                },
                            new CreateAfterSaleServiceActivitiesHandler.DealDto
                                {
                                    DealId = 2,
                                    Orders = new[]
                                        {
                                            new CreateAfterSaleServiceActivitiesHandler.OrderDto
                                                {
                                                    BeginDistributionDate = BeginDistributionDate,
                                                    ReleaseCountFact = 4
                                                }
                                        }
                                }
                        });
                ExistingASSes.Add(1,
                                  new List<AfterSaleServiceActivity>
                                          {
                                              new AfterSaleServiceActivity
                                                  {
                                                      DealId = 1,
                                                      AbsoluteMonthNumber = ReleaseNumber + 1,
                                                      AfterSaleServiceType = (byte)AfterSaleServiceType.ASS3,
                                                  }
                                          });
            };

            It should_be_six_activities_in_total = () => Result.Should().HaveCount(6);
            
            It first_activity_of_first_deal_should_be_of_ASS1_type = 
                () => Result.OfDeal(1).OfReleseNumber(ReleaseNumber).OfAfterSaleServiceType(AfterSaleServiceType.ASS1).Should().HaveCount(1);
            
            It second_activity_of_first_deal_should_be_of_ASS4_type = 
                () => Result.OfDeal(1).OfReleseNumber(ReleaseNumber + 2).OfAfterSaleServiceType(AfterSaleServiceType.ASS4).Should().HaveCount(1);

            It first_activity_of_second_deal_should_be_of_ASS1_type =
                () => Result.OfDeal(2).OfReleseNumber(ReleaseNumber).OfAfterSaleServiceType(AfterSaleServiceType.ASS1).Should().HaveCount(1);

            It second_activity_of_second_deal_should_be_of_ASS2_type =
                () => Result.OfDeal(2).OfReleseNumber(ReleaseNumber + 1).OfAfterSaleServiceType(AfterSaleServiceType.ASS2).Should().HaveCount(1);

            It third_activity_of_second_deal_should_be_of_ASS3_type =
                () => Result.OfDeal(2).OfReleseNumber(ReleaseNumber + 2).OfAfterSaleServiceType(AfterSaleServiceType.ASS3).Should().HaveCount(1);

            It fourth_activity_of_second_deal_should_be_of_ASS4_type =
                () => Result.OfDeal(2).OfReleseNumber(ReleaseNumber + 3).OfAfterSaleServiceType(AfterSaleServiceType.ASS4).Should().HaveCount(1);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class AfterSaleServiceActivityListExtensions
    {
        public static List<AfterSaleServiceActivity> OfReleseNumber(this List<AfterSaleServiceActivity> source, int releaseNumber)
        {
            return source.Where(x => x.AbsoluteMonthNumber == releaseNumber).ToList();
        }

        public static List<AfterSaleServiceActivity> OfAfterSaleServiceType(this List<AfterSaleServiceActivity> source, AfterSaleServiceType afterSaleServiceType)
        {
            return source.Where(x => (AfterSaleServiceType)x.AfterSaleServiceType == afterSaleServiceType).ToList();
        }

        public static List<AfterSaleServiceActivity> OfDeal(this List<AfterSaleServiceActivity> source, long dealId)
        {
            return source.Where(x => x.DealId == dealId).ToList();
        }
    }
}