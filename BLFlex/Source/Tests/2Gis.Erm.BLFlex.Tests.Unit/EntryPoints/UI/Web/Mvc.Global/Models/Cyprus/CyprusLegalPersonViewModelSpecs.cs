using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Cyprus
{
    public class CyprusLegalPersonViewModelSpecs
    {
        public abstract class CyprusLegalPersonViewModelContext
        {
            public static CyprusLegalPersonViewModel Target;
            public static LegalPersonDomainEntityDto DomainEntityDto;
        }

        [Tags("ViewModel")]
        [Subject(typeof(CyprusLegalPersonViewModel))]
        public class When_loading_domain_entity_dto : CyprusLegalPersonViewModelContext
        {
            Establish context = () =>
            {
                Target = new CyprusLegalPersonViewModel();
                DomainEntityDto = new LegalPersonDomainEntityDto();
                LegalPersonViewModelsTestHelper.FillCyprusLegalPersonDtoWithTestData(DomainEntityDto);
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_id = () => Target.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => Target.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => Target.Client.Key.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_LegalAddress = () => Target.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => Target.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_HasProfiles = () => Target.HasProfiles.Should().Be(LegalPersonViewModelsTestHelper.TestHasProfiles);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_Vat = () => Target.VAT.Should().Be(LegalPersonViewModelsTestHelper.TestVat);
            It should_have_expected_PassportNumber = () => Target.PassportNumber.Should().Be(LegalPersonViewModelsTestHelper.TestPassportNumber);
            It should_have_expected_PassportIssuedBy = () => Target.PassportIssuedBy.Should().Be(LegalPersonViewModelsTestHelper.TestPassportIssuedBy);
            It should_have_expected_RegistrationAddress = () => Target.RegistrationAddress.Should().Be(LegalPersonViewModelsTestHelper.TestRegistrationAddress);
            It should_have_expected_CardNumber = () => Target.CardNumber.Should().Be(LegalPersonViewModelsTestHelper.TestCardNumber);
        }


        [Tags("ViewModel")]
        [Subject(typeof(CyprusLegalPersonViewModel))]
        public class When_loading_domain_entity_dto_with_legal_person_type : CyprusLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.LegalPerson;

            Establish context = () =>
            {
                Target = new CyprusLegalPersonViewModel();
                DomainEntityDto = new LegalPersonDomainEntityDto
                {
                    LegalPersonTypeEnum = TestLegalPersonType,
                    Inn = LegalPersonViewModelsTestHelper.TestInn
                };
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(TestLegalPersonType);
            It should_have_expected_Inn = () => Target.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_null_BusinessmanInn = () => Target.BusinessmanInn.Should().BeNull();
        }

        [Tags("ViewModel")]
        [Subject(typeof(CyprusLegalPersonViewModel))]
        public class When_loading_domain_entity_dto_with_businessman_type : CyprusLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;

            Establish context = () =>
                {
                    Target = new CyprusLegalPersonViewModel();
                    DomainEntityDto = new LegalPersonDomainEntityDto
                        {
                            LegalPersonTypeEnum = TestLegalPersonType,
                            Inn = LegalPersonViewModelsTestHelper.TestInn
                        };
                };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(TestLegalPersonType);
            It should_have_expected_BusinessmanInn = () => Target.BusinessmanInn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_null_Inn = () => Target.Inn.Should().BeNull();
        }


        [Tags("ViewModel")]
        [Subject(typeof(CyprusLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto : CyprusLegalPersonViewModelContext
        {
            Establish context = () =>
                {
                    Target = new CyprusLegalPersonViewModel();
                    LegalPersonViewModelsTestHelper.FillCyprusLegalPersonModelWithTestData(Target);
                };

            Because of = () => DomainEntityDto =(LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => DomainEntityDto.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => DomainEntityDto.ClientRef.Id.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_LegalAddress = () => DomainEntityDto.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => DomainEntityDto.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_Vat = () => DomainEntityDto.VAT.Should().Be(LegalPersonViewModelsTestHelper.TestVat);
            It should_have_expected_PassportNumber = () => DomainEntityDto.PassportNumber.Should().Be(LegalPersonViewModelsTestHelper.TestPassportNumber);
            It should_have_expected_PassportIssuedBy = () => DomainEntityDto.PassportIssuedBy.Should().Be(LegalPersonViewModelsTestHelper.TestPassportIssuedBy);
            It should_have_expected_RegistrationAddress = () => DomainEntityDto.RegistrationAddress.Should().Be(LegalPersonViewModelsTestHelper.TestRegistrationAddress);
            It should_have_expected_CardNumber = () => DomainEntityDto.CardNumber.Should().Be(LegalPersonViewModelsTestHelper.TestCardNumber);
        }

        [Tags("ViewModel")]
        [Subject(typeof(CyprusLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto_with_legal_person_type : CyprusLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.LegalPerson;

            Establish context = () =>
            {
                Target = new CyprusLegalPersonViewModel
                {
                    Inn = LegalPersonViewModelsTestHelper.TestInn,
                    BusinessmanInn = null,
                    LegalPersonType = TestLegalPersonType
                };
            };

            Because of = () => DomainEntityDto = (LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(TestLegalPersonType);
            It should_have_expected_Inn = () => DomainEntityDto.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
        }

        [Tags("ViewModel")]
        [Subject(typeof(CyprusLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto_with_businessman_type : CyprusLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;

            Establish context = () =>
            {
                Target = new CyprusLegalPersonViewModel
                {
                    Inn = null,
                    BusinessmanInn = LegalPersonViewModelsTestHelper.TestInn,
                    LegalPersonType = TestLegalPersonType
                };
            };

            Because of = () => DomainEntityDto = (LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(TestLegalPersonType);
            It should_have_expected_Inn = () => DomainEntityDto.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
        }
    }
}
