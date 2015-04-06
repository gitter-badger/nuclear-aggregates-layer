using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Chile
{
    public class ChileLegalPersonProfileViewModelSpecs
    {
        [Tags("ViewModel")]
        [Subject(typeof(ChileLegalPersonProfileViewModel))]
        public class When_loading_domain_entity_dto
        {
            static ChileLegalPersonProfileViewModel Target;
            static ChileLegalPersonProfileDomainEntityDto DomainEntityDto;

            Establish context = () =>
                {
                    Target = new ChileLegalPersonProfileViewModel();
                    DomainEntityDto = new ChileLegalPersonProfileDomainEntityDto();
                    LegalPersonViewProfileModelsTestHelper.FillChileLegalPersonProfileDtoWithTestData(DomainEntityDto);
                };

            Because of = () => Target.LoadDomainEntityDto(DomainEntityDto);

            It should_have_expected_id = () => Target.Id.Should().Be(LegalPersonViewProfileModelsTestHelper.TestId);
            It should_have_expected_Name = () => Target.Name.Should().Be(LegalPersonViewProfileModelsTestHelper.TestName);
            It should_have_expected_LegalPersonId = () => Target.LegalPerson.Key.Should().Be(LegalPersonViewProfileModelsTestHelper.TestLegalPerson.Id);
            It should_have_expected_DocumentsDeliveryAddress = () => Target.DocumentsDeliveryAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestDocumentsDeliveryAddress);
            It should_have_expected_RecipientName = () => Target.RecipientName.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRecipientName);
            It should_have_expected_DocumentsDeliveryMethod = () => Target.DocumentsDeliveryMethod.Should().Be(LegalPersonViewProfileModelsTestHelper.TestDocumentsDeliveryMethod);
            It should_have_expected_PersonResponsibleForDocuments = () => Target.PersonResponsibleForDocuments.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPersonResponsibleForDocuments);
            It should_have_expected_EmailForAccountingDocuments = () => Target.EmailForAccountingDocuments.Should().Be(LegalPersonViewProfileModelsTestHelper.TestEmailForAccountingDocuments);
            It should_have_expected_AdditionalEmail = () => Target.Email.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAdditionalEmail);
            It should_have_expected_PostAddress = () => Target.PostAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPostAddress);
            It should_have_expected_PaymentMethod = () => Target.PaymentMethod.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPaymentMethod);
            It should_have_expected_AccountNumber = () => Target.AccountNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAccountNumber);
            It should_have_expected_Phone = () => Target.Phone.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPhone);
            It should_have_expected_OperatesOnTheBasisInGenitive = () => Target.OperatesOnTheBasisInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestOperatesOnTheBasisType);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewProfileModelsTestHelper.TestTimestamp);
            It should_have_expected_IsMainProfile = () => Target.IsMainProfile.Should().Be(LegalPersonViewProfileModelsTestHelper.TestIsMainProfile);
            It should_have_expected_BankAccountType = () => Target.BankAccountType.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAccountType);
            It should_have_expected_BankId = () => Target.Bank.Key.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBank.Id);
            It should_have_expected_RepresentativeName = () => Target.RepresentativeName.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInNominative);
            It should_have_expected_RepresentativePosition = () => Target.RepresentativePosition.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInNominative);
            It should_have_expected_RepresentativeRut = () => Target.RepresentativeRut.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRepresentativeRut);
            It should_have_expected_RepresentativeDocumentIssuedOn = () => Target.RepresentativeDocumentIssuedOn.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRepresentativeDocumentIssuedOn);
            It should_have_expected_RepresentativeDocumentIssuedBy = () => Target.RepresentativeDocumentIssuedBy.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRepresentativeDocumentIssuedBy);
        }

        [Tags("ViewModel")]
        [Subject(typeof(ChileLegalPersonProfileViewModel))]
        public class When_transforming_to_domain_entity_dto
        {
            static ChileLegalPersonProfileViewModel Target;
            static ChileLegalPersonProfileDomainEntityDto DomainEntityDto;

            Establish context = () =>
                {
                    Target = new ChileLegalPersonProfileViewModel();
                    LegalPersonViewProfileModelsTestHelper.FillChileViewModelWithTestData(Target);
                };

            Because of = () => DomainEntityDto = (ChileLegalPersonProfileDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewProfileModelsTestHelper.TestId);
            It should_have_expected_Name = () => DomainEntityDto.Name.Should().Be(LegalPersonViewProfileModelsTestHelper.TestName);
            It should_have_expected_LegalPersonId = () => DomainEntityDto.LegalPersonRef.Id.Should().Be(LegalPersonViewProfileModelsTestHelper.TestLegalPerson.Id);
            It should_have_expected_DocumentsDeliveryAddress = () => DomainEntityDto.DocumentsDeliveryAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestDocumentsDeliveryAddress);
            It should_have_expected_RecipientName = () => DomainEntityDto.RecipientName.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRecipientName);
            It should_have_expected_DocumentsDeliveryMethod = () => DomainEntityDto.DocumentsDeliveryMethod.Should().Be(LegalPersonViewProfileModelsTestHelper.TestDocumentsDeliveryMethod);
            It should_have_expected_PersonResponsibleForDocuments = () => DomainEntityDto.PersonResponsibleForDocuments.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPersonResponsibleForDocuments);
            It should_have_expected_EmailForAccountingDocuments = () => DomainEntityDto.EmailForAccountingDocuments.Should().Be(LegalPersonViewProfileModelsTestHelper.TestEmailForAccountingDocuments);
            It should_have_expected_AdditionalEmail = () => DomainEntityDto.Email.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAdditionalEmail);
            It should_have_expected_PostAddress = () => DomainEntityDto.PostAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPostAddress);
            It should_have_expected_PaymentMethod = () => DomainEntityDto.PaymentMethod.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPaymentMethod);
            It should_have_expected_AccountNumber = () => DomainEntityDto.AccountNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAccountNumber);
            It should_have_expected_Phone = () => DomainEntityDto.Phone.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPhone);
            It should_have_expected_OperatesOnTheBasisInGenitive = () => DomainEntityDto.OperatesOnTheBasisInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestOperatesOnTheBasisType);
            It should_have_expected_PositionInNominative = () => DomainEntityDto.PositionInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInNominative);
            It should_have_expected_ChiefNameInNominative = () => DomainEntityDto.ChiefNameInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInNominative);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewProfileModelsTestHelper.TestTimestamp);
            It should_have_expected_BankAccountType = () => DomainEntityDto.AccountType.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAccountType);
            It should_have_expected_BankId = () => DomainEntityDto.BankRef.Id.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBank.Id);
            It should_have_expected_RepresentativeRut = () => DomainEntityDto.RepresentativeRut.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRepresentativeRut);
            It should_have_expected_RepresentativeDocumentIssuedOn = () => DomainEntityDto.RepresentativeDocumentIssuedOn.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRepresentativeDocumentIssuedOn);
            It should_have_expected_RepresentativeDocumentIssuedBy = () => DomainEntityDto.RepresentativeDocumentIssuedBy.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRepresentativeDocumentIssuedBy);
        }
    }
}
