using System;
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
    public class CorrectAddressOrderPositionAdvertisementValidationRuleSpecs
    {
        [Tags("BL")]
        [Tags("OrderPositionAdvertisementValidation")]
        [Subject(typeof(CorrectAddressOrderPositionAdvertisementValidationRule))]
        private abstract class BasicCntext
        {
            private const long SponsoredLinkPositionCategoryId = 11;
            private const long AdvantageousPurchasePositionCategoryId = 14;
            private const long AddressCommentPositionCategoryId = 26;

            private Establish context = () =>
                {
                    SponsoredLinkPosition = new Position
                        {
                            Id = 1,
                            Name = "SponsoredLinkPosition",
                            CategoryId = SponsoredLinkPositionCategoryId
                        };

                    AdvantageousPurchasePosition = new Position
                        {
                            Id = 2,
                            Name = "AdvantageousPurchasePosition",
                            CategoryId = AdvantageousPurchasePositionCategoryId
                        };

                    AddressCommentPosition = new Position
                        {
                            Id = 3,
                            Name = "AddressCommentPosition",
                            CategoryId = AddressCommentPositionCategoryId
                        };

                    NormalPosition = new Position
                        {
                            Id = 4,
                            Name = "NormalPosition",
                            CategoryId = 1
                        };

                    HiddenFirmAddress = new FirmAddress
                        {
                            Id = 1,
                            IsActive = false,
                            IsLocatedOnTheMap = true
                        };

                    EmptyFirmAddress = new FirmAddress
                        {
                            Id = 2,
                            IsActive = true,
                            IsLocatedOnTheMap = false,
                            Firm = new Firm
                                {
                                    OrganizationUnit = new OrganizationUnit
                                        {
                                            InfoRussiaLaunchDate = DateTime.Today
                                        }
                                }
                        };

                    NormalFirmAddress = new FirmAddress
                        {
                            Id = 3,
                            IsActive = true,
                            IsLocatedOnTheMap = true
                        };

                    NormalAdvertisement = new AdvertisementDescriptor
                        {
                            PositionId = NormalPosition.Id,
                            FirmAddressId = NormalFirmAddress.Id
                        };

                    SponsoredLinkPositionAndEmptyAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = EmptyFirmAddress.Id,
                            PositionId = SponsoredLinkPosition.Id
                        };

                    AdvantageousPurchasePositionAndEmptyAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = EmptyFirmAddress.Id,
                            PositionId = AdvantageousPurchasePosition.Id
                        };

                    AddressCommentPositionAndEmptyAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = EmptyFirmAddress.Id,
                            PositionId = AddressCommentPosition.Id
                        };

                    NormalPositionAndEmptyAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = EmptyFirmAddress.Id,
                            PositionId = NormalPosition.Id
                        };

                    SponsoredLinkPositionAndHiddenAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = HiddenFirmAddress.Id,
                            PositionId = SponsoredLinkPosition.Id
                        };

                    AdvantageousPurchasePositionAndHiddenAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = HiddenFirmAddress.Id,
                            PositionId = AdvantageousPurchasePosition.Id
                        };

                    AddressCommentPositionAndHiddenAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = HiddenFirmAddress.Id,
                            PositionId = AddressCommentPosition.Id
                        };

                    NormalPositionAndHiddenAddressAdvertisement = new AdvertisementDescriptor
                        {
                            FirmAddressId = HiddenFirmAddress.Id,
                            PositionId = NormalPosition.Id
                        };

                    AdvertisementDescriptorWithoutFirmAddressId = new AdvertisementDescriptor
                        {
                            FirmAddressId = null,
                            PositionId = NormalPosition.Id
                        };

                    FinderMock = new Mock<IFinder>();
                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<FirmAddress>>()))
                              .Returns(
                                  (FindSpecification<FirmAddress> x) =>
                                  new QueryableFutureSequence<FirmAddress>(new[]
                                      {
                                          HiddenFirmAddress,
                                          EmptyFirmAddress,
                                          NormalFirmAddress
                                      }.AsQueryable().Where(x)));

                    FinderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Position>>()))
                              .Returns(
                                       (FindSpecification<Position> x) =>
                                       new QueryableFutureSequence<Position>(new[]
                                           {
                                               SponsoredLinkPosition,
                                               AdvantageousPurchasePosition,
                                               AddressCommentPosition,
                                               NormalPosition
                                           }
                                           .AsQueryable()
                                           .Where(x)));

                    ValidationRule = new CorrectAddressOrderPositionAdvertisementValidationRule(FinderMock.Object);
                };

            private Because of = () =>
                {
                    ValidationErrors = new List<OrderPositionAdvertisementValidationError>();
                    ValidationRule.Validate(AdvertisementToValidate, ValidationErrors);
                };

            protected static AdvertisementDescriptor AdvertisementToValidate { get; set; }

            protected static List<OrderPositionAdvertisementValidationError> ValidationErrors { get; private set; }

            protected static Position SponsoredLinkPosition { get; private set; }
            protected static Position AdvantageousPurchasePosition { get; private set; }
            protected static Position AddressCommentPosition { get; private set; }
            protected static Position NormalPosition { get; private set; }

            protected static AdvertisementDescriptor NormalAdvertisement { get; private set; }
            protected static AdvertisementDescriptor SponsoredLinkPositionAndEmptyAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor AdvantageousPurchasePositionAndEmptyAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor AddressCommentPositionAndEmptyAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor NormalPositionAndEmptyAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor NormalPositionAndHiddenAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor SponsoredLinkPositionAndHiddenAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor AdvantageousPurchasePositionAndHiddenAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor AddressCommentPositionAndHiddenAddressAdvertisement { get; private set; }
            protected static AdvertisementDescriptor AdvertisementDescriptorWithoutFirmAddressId { get; private set; }

            private static IAdvertisementValidationRule ValidationRule { get; set; }

            private static Mock<IFinder> FinderMock { get; set; }

            private static FirmAddress HiddenFirmAddress { get; set; }
            private static FirmAddress EmptyFirmAddress { get; set; }
            private static FirmAddress NormalFirmAddress { get; set; }
        }

        private class When_firmaddress_id_is_not_specified : BasicCntext
        {
            private Establish context = () => AdvertisementToValidate = AdvertisementDescriptorWithoutFirmAddressId;
            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_valid : BasicCntext
        {
            private Establish context = () => AdvertisementToValidate = NormalAdvertisement;
            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_with_empty_address_and_sponsored_link_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = SponsoredLinkPositionAndEmptyAddressAdvertisement;

           private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_with_empty_address_and_advantageous_purchase_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = AdvantageousPurchasePositionAndEmptyAddressAdvertisement;

            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_with_empty_address_and_address_comment_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = AddressCommentPositionAndEmptyAddressAdvertisement;

            private It there_should_be_no_errors = () => ValidationErrors.Should().BeEmpty();
        }

        private class When_advertisement_is_with_empty_address_and_normal_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = NormalPositionAndEmptyAddressAdvertisement;

            private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.CorrectAddress);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.InvalidAddressIsPickedForPosition, NormalPosition.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }

        private class When_advertisements_is_with_hidden_address_and_normal_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = NormalPositionAndHiddenAddressAdvertisement;

            private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.CorrectAddress);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.InvalidAddressIsPickedForPosition, NormalPosition.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }

        private class When_advertisements_is_with_hidden_address_and_sponsored_link_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = SponsoredLinkPositionAndHiddenAddressAdvertisement;

            private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.CorrectAddress);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.InvalidAddressIsPickedForPosition, SponsoredLinkPosition.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }

        private class When_advertisements_is_with_hidden_address_and_advantageous_purchase_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = AdvantageousPurchasePositionAndHiddenAddressAdvertisement;

            private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.CorrectAddress);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.InvalidAddressIsPickedForPosition, AdvantageousPurchasePosition.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }

        private class When_advertisements_is_with_hidden_address_and_address_comment_position : BasicCntext
        {
            private Establish context =
                () =>
                AdvertisementToValidate = AddressCommentPositionAndHiddenAddressAdvertisement;

                        private It errors_should_contain_one_error = () => ValidationErrors.Count.Should().Be(1);
            private It rule_should_be_specified_correct = () => ValidationErrors.First().Rule.Should().Be(OrderPositionAdvertisementValidationRule.CorrectAddress);

            private It error_text_should_describe_error =
                () =>
                ValidationErrors.First().ErrorMessage.Should()
                                .Be(string.Format(BLResources.InvalidAddressIsPickedForPosition, AddressCommentPosition.Name));

            private It validation_result_should_provide_validated_advertisement_descriptor =
                () => ValidationErrors.First().Advertisement.Should().BeSameAs(AdvertisementToValidate);
        }
    }
}
