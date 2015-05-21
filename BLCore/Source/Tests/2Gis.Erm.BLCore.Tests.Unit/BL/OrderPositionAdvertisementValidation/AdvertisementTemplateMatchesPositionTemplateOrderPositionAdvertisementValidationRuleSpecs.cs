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
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.OrderPositionAdvertisementValidation
{
    public class AdvertisementTemplateMatchesPositionTemplateOrderPositionAdvertisementValidationRuleSpecs
    {
        [Tags("BL")]
        [Tags("OrderPositionAdvertisementValidation")]
        [Subject(typeof(AdvertisementTemplateMatchesPositionTemplateOrderPositionAdvertisementValidationRule))]
        private abstract class BasicCntext
        {
            private Establish context = () =>
                {
                    FirstAdvertisementTemplate = new AdvertisementTemplate
                        {
                            Id = 1,
                            Name = "FirstTemplate",
                        };

                    SecondAdvertisementTemplate = new AdvertisementTemplate
                        {
                            Id = 2,
                            Name = "SecondTemplate",
                        };

                    PositionWithFirstTemplate = new Position
                        {
                            Id = 1,
                            AdvertisementTemplateId = FirstAdvertisementTemplate.Id,
                            Name = "PositionWithFirstTemplate"
                        };

                    AdvertisementWithFirstTemplate = new Advertisement
                        {
                            Id = 1,
                            AdvertisementTemplateId = FirstAdvertisementTemplate.Id
                        };

                    AdvertisementWithSecondTemplate = new Advertisement
                        {
                            Id = 2,
                            AdvertisementTemplateId = SecondAdvertisementTemplate.Id
                        };

                    ValidAdvertisementDescriptor = new AdvertisementDescriptor
                        {
                            PositionId = PositionWithFirstTemplate.Id,
                            AdvertisementId = AdvertisementWithFirstTemplate.Id
                        };

                    InvalidAdvertisementDescriptor = new AdvertisementDescriptor
                        {
                            PositionId = PositionWithFirstTemplate.Id,
                            AdvertisementId = AdvertisementWithSecondTemplate.Id
                        };

                    AdvertisementDescriptorWithoutAdvertisementId = new AdvertisementDescriptor
                        {
                            PositionId = PositionWithFirstTemplate.Id,
                            AdvertisementId = null
                        };

                    FinderMock = new Mock<IFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Advertisement>>()))
                              .Returns(
                                       (FindSpecification<Advertisement> x) =>
                                       new[]
                                           {
                                               AdvertisementWithFirstTemplate,
                                               AdvertisementWithSecondTemplate
                                           }
                                           .AsQueryable()
                                           .Where(x));

                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Position>>()))
                              .Returns(
                                       (FindSpecification<Position> x) =>
                                       new[]
                                           {
                                               PositionWithFirstTemplate
                                           }
                                           .AsQueryable()
                                           .Where(x));

                    ValidationRule = new AdvertisementTemplateMatchesPositionTemplateOrderPositionAdvertisementValidationRule(FinderMock.Object);
                };

            private Because of = () =>
                {
                    ValidationErrors = new List<OrderPositionAdvertisementValidationError>();
                    ValidationRule.Validate(AdvertisementToValidate, ValidationErrors);
                };

            protected static AdvertisementDescriptor ValidAdvertisementDescriptor { get; private set; }
            protected static AdvertisementDescriptor InvalidAdvertisementDescriptor { get; private set; }
            protected static AdvertisementDescriptor AdvertisementDescriptorWithoutAdvertisementId { get; private set; }

            protected static AdvertisementDescriptor AdvertisementToValidate { get; set; }

            protected static List<OrderPositionAdvertisementValidationError> ValidationErrors { get; private set; }
            protected static Position PositionWithFirstTemplate { get; private set; }

            private static Advertisement AdvertisementWithFirstTemplate { get; set; }
            private static Advertisement AdvertisementWithSecondTemplate { get; set; }
            private static AdvertisementTemplate FirstAdvertisementTemplate { get; set; }
            private static AdvertisementTemplate SecondAdvertisementTemplate { get; set; }

            private static IAdvertisementValidationRule ValidationRule { get; set; }

            private static Mock<IFinder> FinderMock { get; set; }
        }

        private class When_advertisement_is_valid : BasicCntext
        {
            private Establish context = () => AdvertisementToValidate = ValidAdvertisementDescriptor;
            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_id_is_not_specified : BasicCntext
        {
            private Establish context = () => AdvertisementToValidate = AdvertisementDescriptorWithoutAdvertisementId;
            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_invalid : BasicCntext
        {
            private Establish context =
                () => AdvertisementToValidate = InvalidAdvertisementDescriptor;

            private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.AdvertisementTemplateMatchesPositionTemplate);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.AdvertisementTemplateDifferentFromPositionAdvertisementTemplate,
                                                  PositionWithFirstTemplate.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }
    }
}
