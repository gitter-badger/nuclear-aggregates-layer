using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Czech
{
    public class CzechLegalPersonViewModelSpecs
    {
        public abstract class CzechLegalPersonViewModelContext
        {
            public static CzechLegalPersonViewModel Target;
            public static LegalPersonDomainEntityDto DomainEntityDto;
        }

        [Tags("ViewModel")]
        [Subject(typeof(CzechLegalPersonViewModel))]
        public class When_loading_domain_entity_dto : CzechLegalPersonViewModelContext
        {
            Establish context = () =>
            {
                Target = new CzechLegalPersonViewModel();
                DomainEntityDto = new LegalPersonDomainEntityDto();
                LegalPersonViewModelsTestHelper.FillCzechLegalPersonDtoWithTestData(DomainEntityDto);
            };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_id = () => Target.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => Target.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => Target.Client.Key.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_LegalAddress = () => Target.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => Target.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_HasProfiles = () => Target.HasProfiles.Should().Be(LegalPersonViewModelsTestHelper.TestHasProfiles);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_Ic = () => Target.Ic.Should().Be(LegalPersonViewModelsTestHelper.TestIc);
            It should_have_expected_Inn = () => Target.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
        }

        [Tags("ViewModel")]
        [Subject(typeof(CzechLegalPersonViewModel))]
        public class When_transforming_to_domain_entity_dto : CzechLegalPersonViewModelContext
        {
            Establish context = () =>
                {
                    Target = new CzechLegalPersonViewModel();
                    LegalPersonViewModelsTestHelper.FillCzechLegalPersonModelWithTestData(Target);
                };

            Because of = () => DomainEntityDto = (LegalPersonDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewModelsTestHelper.TestId);
            It should_have_expected_LegalName = () => DomainEntityDto.LegalName.Should().Be(LegalPersonViewModelsTestHelper.TestLegalName);
            It should_have_expected_ClientId = () => DomainEntityDto.ClientRef.Id.Should().Be(LegalPersonViewModelsTestHelper.TestClient.Id);
            It should_have_expected_LegalAddress = () => DomainEntityDto.LegalAddress.Should().Be(LegalPersonViewModelsTestHelper.TestLegalAddress);
            It should_have_expected_Comment = () => DomainEntityDto.Comment.Should().Be(LegalPersonViewModelsTestHelper.TestComment);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewModelsTestHelper.TestTimestamp);
            It should_have_expected_Ic = () => DomainEntityDto.Ic.Should().Be(LegalPersonViewModelsTestHelper.TestIc);
            It should_have_expected_Inn = () => DomainEntityDto.Inn.Should().Be(LegalPersonViewModelsTestHelper.TestInn);
        }
    }
}