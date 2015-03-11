using DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Russia
{
    public class LegalPersonProfileViewModelSpecs
    {
        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonProfileViewModel))]
        public class When_loading_domain_entity_dto
        {
            static LegalPersonProfileViewModel Target;
            static LegalPersonProfileDomainEntityDto DomainEntityDto;

            Establish context = () =>
            {
                Target = new LegalPersonProfileViewModel();
                DomainEntityDto = new LegalPersonProfileDomainEntityDto();
                LegalPersonViewProfileModelsTestHelper.FillRussiaLegalPersonProfileDtoWithTestData(DomainEntityDto);
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
            It should_have_expected_Phone = () => Target.Phone.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPhone);
            It should_have_expected_OperatesOnTheBasisInGenitive = () => Target.OperatesOnTheBasisInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestOperatesOnTheBasisType);
            It should_have_expected_Timestamp = () => Target.Timestamp.Should().BeEquivalentTo(LegalPersonViewProfileModelsTestHelper.TestTimestamp);
            It should_have_expected_IsMainProfile = () => Target.IsMainProfile.Should().Be(LegalPersonViewProfileModelsTestHelper.TestIsMainProfile);
            It should_have_expected_PositionInGenitive = () => Target.PositionInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInGenitive);
            It should_have_expected_PositionInNominative = () => Target.PositionInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInNominative);
            It should_have_expected_ChiefNameInNominative = () => Target.ChiefNameInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInNominative);
            It should_have_expected_ChiefNameInGenitive = () => Target.ChiefNameInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInGenitive);
            It should_have_expected_CertificateNumber = () => Target.CertificateNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestCertificateNumber);
            It should_have_expected_CertificateDate = () => Target.CertificateDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestCertificateDate);
            It should_have_expected_WarrantyNumber = () => Target.WarrantyNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyNumber);
            It should_have_expected_WarrantyBeginDate = () => Target.WarrantyBeginDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyBeginDate);
            It should_have_expected_WarrantyEndDate = () => Target.WarrantyEndDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyEndDate);
            It should_have_expected_BargainNumber = () => Target.BargainNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBargainNumber);
            It should_have_expected_BargainBeginDate = () => Target.BargainBeginDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBargainBeginDate);
            It should_have_expected_BargainEndDate = () => Target.BargainEndDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBargainEndDate);
            It should_have_expected_PaymentEssentialElements = () => Target.PaymentEssentialElements.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPaymentEssentialElements);
        }

        [Tags("ViewModel")]
        [Subject(typeof(LegalPersonProfileViewModel))]
        public class When_transforming_to_domain_entity_dto
        {
            static LegalPersonProfileViewModel Target;
            static LegalPersonProfileDomainEntityDto DomainEntityDto;

            Establish context = () =>
                {
                    Target = new LegalPersonProfileViewModel();
                    LegalPersonViewProfileModelsTestHelper.FillRussiaViewModelWithTestData(Target);
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
            It should_have_expected_AdditionalEmail = () => DomainEntityDto.Email.Should().Be(LegalPersonViewProfileModelsTestHelper.TestAdditionalEmail);
            It should_have_expected_PostAddress = () => DomainEntityDto.PostAddress.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPostAddress);
            It should_have_expected_PaymentEssentialElements = () => DomainEntityDto.PaymentEssentialElements.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPaymentEssentialElements);
            It should_have_expected_Phone = () => DomainEntityDto.Phone.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPhone);
            It should_have_expected_OperatesOnTheBasisInGenitive = () => DomainEntityDto.OperatesOnTheBasisInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestOperatesOnTheBasisType);
            It should_have_expected_Timestamp = () => DomainEntityDto.Timestamp.Should().BeEquivalentTo(LegalPersonViewProfileModelsTestHelper.TestTimestamp);
            It should_have_expected_PositionInGenitive = () => DomainEntityDto.PositionInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInGenitive);
            It should_have_expected_PositionInNominative = () => DomainEntityDto.PositionInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestPositionInNominative);
            It should_have_expected_ChiefNameInNominative = () => DomainEntityDto.ChiefNameInNominative.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInNominative);
            It should_have_expected_ChiefNameInGenitive = () => DomainEntityDto.ChiefNameInGenitive.Should().Be(LegalPersonViewProfileModelsTestHelper.TestChiefNameInGenitive);
            It should_have_expected_CertificateNumber = () => DomainEntityDto.CertificateNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestCertificateNumber);
            It should_have_expected_CertificateDate = () => DomainEntityDto.CertificateDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestCertificateDate);
            It should_have_expected_WarrantyNumber = () => DomainEntityDto.WarrantyNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyNumber);
            It should_have_expected_WarrantyBeginDate = () => DomainEntityDto.WarrantyBeginDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyBeginDate);
            It should_have_expected_WarrantyEndDate = () => DomainEntityDto.WarrantyEndDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestWarrantyEndDate);
            It should_have_expected_BargainNumber = () => DomainEntityDto.BargainNumber.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBargainNumber);
            It should_have_expected_BargainBeginDate = () => DomainEntityDto.BargainBeginDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBargainBeginDate);
            It should_have_expected_BargainEndDate = () => DomainEntityDto.BargainEndDate.Should().Be(LegalPersonViewProfileModelsTestHelper.TestBargainEndDate);
        }
    }
}
