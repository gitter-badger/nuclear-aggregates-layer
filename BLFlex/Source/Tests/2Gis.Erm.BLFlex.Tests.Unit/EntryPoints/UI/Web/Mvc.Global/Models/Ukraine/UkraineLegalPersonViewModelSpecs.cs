using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Ukraine
{
    public class UkraineLegalPersonViewModelSpecs
    {
        public abstract class UkraineLegalPersonViewModelContext
        {
            public static UkraineLegalPersonViewModel Target;
            public static UkraineLegalPersonDomainEntityDto DomainEntityDto;
        }

        [Tags("ViewModel")]
        [Subject(typeof(UkraineLegalPersonViewModel))]
        public class When_loading_domain_entity_dto : UkraineLegalPersonViewModelContext
        {
            Establish context = () =>
            {
                Target = new UkraineLegalPersonViewModel();
                DomainEntityDto = new UkraineLegalPersonDomainEntityDto();
                LegalPersonViewModelsTestHelper.FillUkraineLegalPersonDtoWithTestData(DomainEntityDto);
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_id = () => Target.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => Target.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => Target.Client.Key.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_Ipn = () => Target.Ipn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_TaxationType = () => Target.TaxationType.Should().Be(LegalPersonViewModelsTestHelper.TestTaxationType);
            It should_have_expected_LegalAddress = () => Target.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => Target.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_HasProfiles = () => Target.HasProfiles.Should().Be(LegalPersonViewModelsTestHelper.TestHasProfiles);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
        }

        [Tags("ViewModel")]
        [Subject(typeof(UkraineLegalPersonViewModel))]
        public class When_loading_domain_entity_dto_with_legal_person_type : UkraineLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.LegalPerson;

            Establish context = () =>
            {
                Target = new UkraineLegalPersonViewModel();
                DomainEntityDto = new UkraineLegalPersonDomainEntityDto
                {
                    LegalPersonTypeEnum = TestLegalPersonType,
                    Egrpou = LegalPersonViewModelsTestHelper.TestEgrpou
                };
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(TestLegalPersonType);
            It should_have_expected_Egrpou = () => Target.Egrpou.Should().Be(LegalPersonViewModelsTestHelper.TestEgrpou);
            It should_have_expected_null_BusinessmanEgrpou = () => Target.BusinessmanEgrpou.Should().BeNull();
        }

        [Tags("ViewModel")]
        [Subject(typeof(UkraineLegalPersonViewModel))]
        public class When_loading_domain_entity_dto_with_businessman_type : UkraineLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;

            Establish context = () =>
            {
                Target = new UkraineLegalPersonViewModel();
                DomainEntityDto = new UkraineLegalPersonDomainEntityDto
                {
                    LegalPersonTypeEnum = TestLegalPersonType,
                    Egrpou = LegalPersonViewModelsTestHelper.TestEgrpou
                };
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(TestLegalPersonType);
            It should_have_expected_BusinessmanEgrpou = () => Target.BusinessmanEgrpou.Should().Be(LegalPersonViewModelsTestHelper.TestEgrpou);
            It should_have_expected_null_Egrpou = () => Target.Egrpou.Should().BeNull();
        }

        [Tags("ViewModel")]
        [Subject(typeof(UkraineLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto : UkraineLegalPersonViewModelContext
        {
            Establish context = () =>
            {
                Target = new UkraineLegalPersonViewModel();
                LegalPersonViewModelsTestHelper.FillUkraineLegalPersonModelWithTestData(Target);
            };

            Because of = () => DomainEntityDto = (UkraineLegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => DomainEntityDto.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => DomainEntityDto.ClientRef.Id.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_Inn = () => DomainEntityDto.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_TaxationType = () => DomainEntityDto.TaxationType.Should().Be(LegalPersonViewModelsTestHelper.TestTaxationType);
            It should_have_expected_LegalAddress = () => DomainEntityDto.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => DomainEntityDto.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
        }

        [Tags("ViewModel")]
        [Subject(typeof(UkraineLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto_with_legal_person_type : UkraineLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.LegalPerson;

            Establish context = () =>
            {
                Target = new UkraineLegalPersonViewModel
                {
                    Egrpou = LegalPersonViewModelsTestHelper.TestEgrpou,
                    BusinessmanEgrpou = null,
                    LegalPersonType = TestLegalPersonType
                };
            };

            Because of = () => DomainEntityDto = (UkraineLegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(TestLegalPersonType);
            It should_have_expected_Egrpou = () => DomainEntityDto.Egrpou.Should().Be(LegalPersonViewModelsTestHelper.TestEgrpou);
        }

        [Tags("ViewModel")]
        [Subject(typeof(UkraineLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto_with_businessman_type : UkraineLegalPersonViewModelContext
        {
            const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;

            Establish context = () =>
            {
                Target = new UkraineLegalPersonViewModel
                {
                    Egrpou = null,
                    BusinessmanEgrpou = LegalPersonViewModelsTestHelper.TestEgrpou,
                    LegalPersonType = TestLegalPersonType
                };
            };

            Because of = () => DomainEntityDto = (UkraineLegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(TestLegalPersonType);
            It should_have_expected_Egrpou = () => DomainEntityDto.Egrpou.Should().Be(LegalPersonViewModelsTestHelper.TestEgrpou);
        }
    }
}