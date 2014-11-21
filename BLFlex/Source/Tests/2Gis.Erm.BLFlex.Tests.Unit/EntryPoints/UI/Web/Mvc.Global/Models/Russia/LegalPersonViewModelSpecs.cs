using System;

using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Russia
{
    public class LegalPersonViewModelSpecs
    {
        public abstract class LegalPersonViewModelContext
        {
            public static LegalPersonViewModel Target;
            public static LegalPersonDomainEntityDto DomainEntityDto;

            Establish context = () =>
                {
                    Target = new LegalPersonViewModel
                        {
                            // Избегаем NullReferenceException
                            ReplicationCode = new Guid()
                        };

                    DomainEntityDto = new LegalPersonDomainEntityDto
                        {
                            // Избегаем NullReferenceException
                            ReplicationCode = new Guid()
                        };
                };
        }

        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonViewModel))]
        public class When_loading_domain_entity_dto : LegalPersonViewModelContext
        {
            Establish context = () => LegalPersonViewModelsTestHelper.FillRussiaLegalPersonDtoWithTestData(DomainEntityDto);

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_id = () => Target.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => Target.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => Target.Client.Key.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_LegalAddress = () => Target.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => Target.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_HasProfiles = () => Target.HasProfiles.Should().Be(LegalPersonViewModelsTestHelper.TestHasProfiles);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_PassportNumber = () => Target.PassportNumber.Should().Be(LegalPersonViewModelsTestHelper.TestPassportNumber);
            It should_have_expected_PassportIssuedBy = () => Target.PassportIssuedBy.Should().Be(LegalPersonViewModelsTestHelper.TestPassportIssuedBy);
            It should_have_expected_RegistrationAddress = () => Target.RegistrationAddress.Should().Be(LegalPersonViewModelsTestHelper.TestRegistrationAddress);
            It should_have_expected_PassportSeries = () => Target.PassportSeries.Should().Be(LegalPersonViewModelsTestHelper.TestPassportSeries);
            It should_have_expected_Kpp = () => Target.Kpp.Should().Be(LegalPersonViewModelsTestHelper.TestKpp);
        }


        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonViewModel))]
        public class When_loading_domain_entity_dto_with_legal_person_type : LegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.LegalPerson;

            Establish context = () =>
            {
                DomainEntityDto = new LegalPersonDomainEntityDto
                {
                    LegalPersonTypeEnum = TestLegalPersonType,
                    Inn = LegalPersonViewModelsTestHelper.TestInn,

                    // Избегаем NullReferenceException
                    ReplicationCode = new Guid()
                };
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(TestLegalPersonType);
            It should_have_expected_Inn = () => Target.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_null_BusinessmanInn = () => Target.BusinessmanInn.Should().BeNull();
        }

        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonViewModel))]
        public class When_loading_domain_entity_dto_with_businessman_type : LegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;

            Establish context = () =>
                {
                    DomainEntityDto = new LegalPersonDomainEntityDto
                        {
                            LegalPersonTypeEnum = TestLegalPersonType,
                            Inn = LegalPersonViewModelsTestHelper.TestInn,

                            // Избегаем NullReferenceException
                            ReplicationCode = new Guid()
                        };
                };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(TestLegalPersonType);
            It should_have_expected_BusinessmanInn = () => Target.BusinessmanInn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_null_Inn = () => Target.Inn.Should().BeNull();
        }


        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto : LegalPersonViewModelContext
        {
            Establish context = () => LegalPersonViewModelsTestHelper.FillRussiaLegalPersonModelWithTestData(Target);

            Because of = () => DomainEntityDto =(LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => DomainEntityDto.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => DomainEntityDto.ClientRef.Id.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_LegalAddress = () => DomainEntityDto.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => DomainEntityDto.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_Kpp = () => DomainEntityDto.Kpp.Should().Be(LegalPersonViewModelsTestHelper.TestKpp);
            It should_have_expected_PassportNumber = () => DomainEntityDto.PassportNumber.Should().Be(LegalPersonViewModelsTestHelper.TestPassportNumber);
            It should_have_expected_PassportIssuedBy = () => DomainEntityDto.PassportIssuedBy.Should().Be(LegalPersonViewModelsTestHelper.TestPassportIssuedBy);
            It should_have_expected_RegistrationAddress = () => DomainEntityDto.RegistrationAddress.Should().Be(LegalPersonViewModelsTestHelper.TestRegistrationAddress);
            It should_have_expected_PassportSeries = () => DomainEntityDto.PassportSeries.Should().Be(LegalPersonViewModelsTestHelper.TestPassportSeries);
        }

        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto_with_legal_person_type : LegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.LegalPerson;

            Establish context = () =>
            {
                Target = new LegalPersonViewModel
                {
                    Inn = LegalPersonViewModelsTestHelper.TestInn,
                    BusinessmanInn = null,
                    LegalPersonType = TestLegalPersonType,

                    // Избегаем NullReferenceException
                    ReplicationCode = new Guid()
                };
            };

            Because of = () => DomainEntityDto = (LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(TestLegalPersonType);
            It should_have_expected_Inn = () => DomainEntityDto.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
        }

        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto_with_businessman_type : LegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;

            Establish context = () =>
            {
                Target = new LegalPersonViewModel
                {
                    Inn = null,
                    BusinessmanInn = LegalPersonViewModelsTestHelper.TestInn,
                    LegalPersonType = TestLegalPersonType,

                    // Избегаем NullReferenceException
                    ReplicationCode = new Guid()
                };
            };

            Because of = () => DomainEntityDto = (LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(TestLegalPersonType);
            It should_have_expected_Inn = () => DomainEntityDto.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
        }
    }
}
