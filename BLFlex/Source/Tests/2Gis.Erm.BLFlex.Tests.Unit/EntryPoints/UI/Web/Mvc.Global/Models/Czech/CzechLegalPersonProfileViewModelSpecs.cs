using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Czech
{
    public class CzechLegalPersonProfileViewModelSpecs
    {
        [Tags("ViewModel")]
        [Subject(typeof(CzechLegalPersonProfileViewModel))]
        public class When_loading_domain_entity_dto
        {
            static CzechLegalPersonProfileViewModel Target;
            static LegalPersonProfileDomainEntityDto DomainEntityDto;

            Establish context = () =>
            {
                Target = new CzechLegalPersonProfileViewModel();
                DomainEntityDto = new LegalPersonProfileDomainEntityDto();
                LegalPersonViewProfileModelsTestHelper.FillCzechLegalPersonProfileDtoWithTestData(DomainEntityDto);
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
            It should_have_expected_PositionInGenitive = () => Target.PositionInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInGenitive);
            It should_have_expected_PositionInNominative = () => Target.PositionInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInNominative);
            It should_have_expected_ChiefNameInNominative = () => Target.ChiefNameInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInNominative);
            It should_have_expected_ChiefNameInGenitive = () => Target.ChiefNameInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInGenitive);
            It should_have_expected_BankName = () => Target.BankName.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBankName);
            It should_have_expected_WarrantyBeginDate = () => Target.WarrantyBeginDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyBeginDate);            
            It should_have_expected_Registered = () => Target.Registered.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRegistered);
            It should_have_expected_BankCode = () => Target.BankCode.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBankCode);
            It should_have_expected_BankAddress = () => Target.BankAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBankAddress);
        }

        [Tags("ViewModel")]
        [Subject(typeof(CzechLegalPersonProfileViewModel))]
        public class When_transforming_to_domain_entity_dto
        {
            static CzechLegalPersonProfileViewModel Target;
            static LegalPersonProfileDomainEntityDto DomainEntityDto;

            Establish context = () =>
                {
                    Target = new CzechLegalPersonProfileViewModel();
                    LegalPersonViewProfileModelsTestHelper.FillCzechViewModelWithTestData(Target);
                };

            Because of = () => DomainEntityDto = (LegalPersonProfileDomainEntityDto)Target.TransformToDomainEntityDto();

            It should_have_expected_id = () => DomainEntityDto.Id.Should().Be(LegalPersonViewProfileModelsTestHelper.TestId);
            It should_have_expected_Name = () => DomainEntityDto.Name.Should().Be(LegalPersonViewProfileModelsTestHelper.TestName);
            It should_have_expected_LegalPersonId = () => DomainEntityDto.LegalPersonRef.Id.Should().Be(LegalPersonViewProfileModelsTestHelper.TestLegalPerson.Id);
            It should_have_expected_DocumentsDeliveryAddress = () => DomainEntityDto.DocumentsDeliveryAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestDocumentsDeliveryAddress);
            It should_have_expected_RecipientName = () => DomainEntityDto.RecipientName.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRecipientName);
            It should_have_expected_DocumentsDeliveryMethod = () => DomainEntityDto.DocumentsDeliveryMethod.Should().Be(LegalPersonViewProfileModelsTestHelper.TestDocumentsDeliveryMethod);
            It should_have_expected_PersonResponsibleForDocuments = () => DomainEntityDto.PersonResponsibleForDocuments.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPersonResponsibleForDocuments);
            It should_have_expected_EmailForAccountingDocuments = () => DomainEntityDto.EmailForAccountingDocuments.Should().Be(LegalPersonViewProfileModelsTestHelper.TestEmailForAccountingDocuments);
            It should_have_expected_AdditionalEmail = () => DomainEntityDto.AdditionalEmail.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAdditionalEmail);
            It should_have_expected_PostAddress = () => DomainEntityDto.PostAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPostAddress);
            It should_have_expected_PaymentMethod = () => DomainEntityDto.PaymentMethod.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPaymentMethod);
            It should_have_expected_AccountNumber = () => DomainEntityDto.AccountNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAccountNumber);
            It should_have_expected_Phone = () => DomainEntityDto.Phone.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPhone);
            It should_have_expected_OperatesOnTheBasisInGenitive = () => DomainEntityDto.OperatesOnTheBasisInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestOperatesOnTheBasisType);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewProfileModelsTestHelper.TestTimestamp);
            It should_have_expected_PositionInGenitive = () => DomainEntityDto.PositionInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInGenitive);
            It should_have_expected_PositionInNominative = () => DomainEntityDto.PositionInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInNominative);
            It should_have_expected_ChiefNameInNominative = () => DomainEntityDto.ChiefNameInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInNominative);
            It should_have_expected_ChiefNameInGenitive = () => DomainEntityDto.ChiefNameInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInGenitive);
            It should_have_expected_BankName = () => DomainEntityDto.BankName.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBankName);
            It should_have_expected_WarrantyBeginDate = () => DomainEntityDto.WarrantyBeginDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyBeginDate);
            It should_have_expected_Registered = () => DomainEntityDto.Registered.Should().Be(LegalPersonViewProfileModelsTestHelper.TestRegistered);
            It should_have_expected_BankCode = () => DomainEntityDto.BankCode.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBankCode);
            It should_have_expected_BankAddress = () => DomainEntityDto.BankAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBankAddress);
        }
    }
}
