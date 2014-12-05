
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Chile
{
    public class ChileLegalPersonViewModelSpecs
    {
        public abstract class ChileLegalPersonViewModelContext
        {
            public static ChileLegalPersonViewModel Target;
            public static ChileLegalPersonDomainEntityDto DomainEntityDto;
        }

        [Tags("ViewModel")]
        [Subject(typeof(ChileLegalPersonViewModel))]
        public class When_loading_domain_entity_dto : ChileLegalPersonViewModelContext
        {
            Establish context = () =>
                {
                    Target = new ChileLegalPersonViewModel();
                    DomainEntityDto = new ChileLegalPersonDomainEntityDto();
                    LegalPersonViewModelsTestHelper.FillChileLegalPersonDtoWithTestData(DomainEntityDto);
                };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_id = () => Target.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => Target.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => Target.Client.Key.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_Rut = () => Target.Rut.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_LegalAddress = () => Target.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => Target.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_HasProfiles = () => Target.HasProfiles.Should().Be(LegalPersonViewModelsTestHelper.TestHasProfiles);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_LegalPersonType = () => Target.LegalPersonType.Should().Be(LegalPersonViewModelsTestHelper.TestLegalPersonType);
        }

        [Tags("ViewModel")]
        [Subject(typeof(ChileLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto : ChileLegalPersonViewModelContext
        {
            Establish context = () =>
                {
                    Target = new ChileLegalPersonViewModel();
                    LegalPersonViewModelsTestHelper.FillChileLegalPersonModelWithTestData(Target);
                };

            Because of = () => DomainEntityDto = (ChileLegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => DomainEntityDto.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => DomainEntityDto.ClientRef.Id.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_Inn = () => DomainEntityDto.Rut.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
            It should_have_expected_OperationsKind = () => DomainEntityDto.OperationsKind.Should().Be(LegalPersonViewModelsTestHelper.TestOperationsKind);
            It should_have_expected_CommuneId = () => DomainEntityDto.CommuneRef.Id.Should().Be(LegalPersonViewModelsTestHelper.TestCommune.Id);
            It should_have_expected_LegalAddress = () => DomainEntityDto.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => DomainEntityDto.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_LegalPersonType = () => DomainEntityDto.LegalPersonTypeEnum.Should().Be(LegalPersonViewModelsTestHelper.TestLegalPersonType);
        }
    }
}
