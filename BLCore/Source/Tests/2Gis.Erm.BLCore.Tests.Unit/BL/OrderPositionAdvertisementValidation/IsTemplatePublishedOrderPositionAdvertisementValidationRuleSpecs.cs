using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositionAdvertisementValidation.Rules;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderPositionAdvertisementValidation
{
    public class IsTemplatePublishedOrderPositionAdvertisementValidationRuleSpecs
    {
        [Tags("BL")]
        [Tags("OrderPositionAdvertisementValidation")]
        [Subject(typeof(IsTemplatePublishedOrderPositionAdvertisementValidationRule))]
        private abstract class BasicCntext
        {
            private Establish context = () =>
                {
                    TestPosition = new Position
                        {
                            Id = 1,
                            Name = "TestPosition"
                        };

                    AdvertisementWithPublishedTemplate = new Advertisement
                        {
                            Id = 1,
                            AdvertisementTemplate = new AdvertisementTemplate
                                {
                                    IsPublished = true
                                }
                        };

                    AdvertisementWithUnpublishedTemplate = new Advertisement
                        {
                            Id = 2,
                            AdvertisementTemplate = new AdvertisementTemplate
                                {
                                    IsPublished = false
                                }
                        };

                    ValidAdvertisementDescriptor = new AdvertisementDescriptor
                        {
                            AdvertisementId = AdvertisementWithPublishedTemplate.Id,
                            PositionId = TestPosition.Id
                        };

                    InvalidAdvertisementDescriptor = new AdvertisementDescriptor
                        {
                            AdvertisementId = AdvertisementWithUnpublishedTemplate.Id,
                            PositionId = TestPosition.Id
                        };

                    FinderMock = new Mock<IFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Advertisement>>()))
                              .Returns(
                                  (FindSpecification<Advertisement> x) =>
                                  new QueryableFutureSequence<Advertisement>(new[]
                                      {
                                          AdvertisementWithPublishedTemplate,
                                          AdvertisementWithUnpublishedTemplate
                                      }.AsQueryable().Where(x)));

                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Position>>()))
                              .Returns((FindSpecification<Position> x) =>
                                  new QueryableFutureSequence<Position>(new[]
                                      {
                                          TestPosition
                                      }.AsQueryable().Where(x)));

                    ValidationRule = new IsTemplatePublishedOrderPositionAdvertisementValidationRule(FinderMock.Object);
                };

            private Because of = () =>
                {
                    ValidationErrors = new List<OrderPositionAdvertisementValidationError>();
                    ValidationRule.Validate(AdvertisementToValidate, ValidationErrors);
                };

            protected static Position TestPosition { get; private set; }

            protected static AdvertisementDescriptor ValidAdvertisementDescriptor { get; private set; }
            protected static AdvertisementDescriptor InvalidAdvertisementDescriptor { get; private set; }
            protected static AdvertisementDescriptor AdvertisementToValidate { get; set; }

            protected static List<OrderPositionAdvertisementValidationError> ValidationErrors { get; private set; }

            private static Advertisement AdvertisementWithPublishedTemplate { get; set; }
            private static Advertisement AdvertisementWithUnpublishedTemplate { get; set; }

            private static IAdvertisementValidationRule ValidationRule { get; set; }

            private static Mock<IFinder> FinderMock { get; set; }
        }

        private class When_advertisement_is_valid : BasicCntext
        {
            private Establish context = () => AdvertisementToValidate = ValidAdvertisementDescriptor;
            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_invalid : BasicCntext
        {
            private Establish context =
                () => AdvertisementToValidate = InvalidAdvertisementDescriptor;

            private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.IsTemplatePublished);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.AdvertisementWithUnpublishedTemplateIsChosen,
                                                  TestPosition.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }
    }
}
