using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Operations.Generic.Modify.DomainEntityObtainers
{
    public class LegalPersonProfileObtainerSpecs
    {
        public abstract class LegalPersonProfileObtainerContext
        {
            protected static IFinder Finder;
            protected static LegalPersonProfileDomainEntityDto DomainEntityDto;

            protected static LegalPersonProfileObtainer LegalPersonProfileObtainer;

            protected static LegalPersonProfile LegalPersonProfile;

            Establish context = () =>
                {
                    LegalPersonProfile = new LegalPersonProfile();
                    DomainEntityDto = new LegalPersonProfileDomainEntityDto
                        {
                            // Задаем эти свойства, чтобы тесты не падали по NullReferenceException
                            LegalPersonRef = new EntityReference(1, null),
                            OwnerRef = new EntityReference(1, null),
                        };

                    Finder = Mock.Of<IFinder>();

                    LegalPersonProfileObtainer = new LegalPersonProfileObtainer(Finder);
                };
        }

        public class LegalPersonProfileObtainerResultContext : LegalPersonProfileObtainerContext
        {
            protected static LegalPersonProfile Result;

            Because of = () => { Result = LegalPersonProfileObtainer.ObtainBusinessModelEntity(DomainEntityDto); };
        }

        [Tags("Obtainer")]
        [Subject(typeof(LegalPersonProfileObtainer))]
        public class When_entity_does_not_exist_in_persistance : LegalPersonProfileObtainerResultContext
        {
            Establish context = () =>
                {
                    DomainEntityDto.Id = 0; // Признак того, что создаем новую сущность
                };

            It should_return_active_entity = () => Result.IsActive.Should().BeTrue();
            It should_return_not_deleted_entity = () => Result.IsDeleted.Should().BeFalse();
        }

        [Tags("Obtainer")]
        [Subject(typeof(LegalPersonProfileObtainer))]
        public class When_entity_exists_in_persistance : LegalPersonProfileObtainerResultContext
        {
            private const long CurrentCreatedBy = 345;
            private const long CurrentModifiedBy = 456;
            private static readonly EntityReference TestCreatedBy = new EntityReference(CurrentCreatedBy + 1, "TestCreatedBy");
            private static readonly EntityReference TestModifiedBy = new EntityReference(CurrentModifiedBy + 1, "TestModifieddBy");
            private const bool TestIsActive = true;
            private const bool TestIsDeleted = true;
            private const bool TestIsMainProfile = true;
            private static readonly DateTime CurrentCreatedOn = DateTime.UtcNow;
            private static readonly DateTime CurrentModifiedOn = DateTime.UtcNow;
            private static readonly DateTime TestCreatedOn = CurrentCreatedOn.AddMonths(1);
            private static readonly DateTime TestModifiedOn = CurrentModifiedOn.AddMonths(2);

            private Establish context = () =>
                {
                    Mock.Get(Finder).Setup(x => x.FindOne(Moq.It.IsAny<IFindSpecification<LegalPersonProfile>>())).Returns(LegalPersonProfile);

                    DomainEntityDto.Id = 1; // Id != 0 => сущность создадим не на new, а возьмем из хранилища

                    DomainEntityDto.CreatedByRef = TestCreatedBy;
                    DomainEntityDto.ModifiedByRef = TestModifiedBy;
                    DomainEntityDto.IsActive = TestIsActive;
                    DomainEntityDto.IsDeleted = TestIsDeleted;
                    DomainEntityDto.ModifiedOn = TestModifiedOn;
                    DomainEntityDto.CreatedOn = TestCreatedOn;
                    DomainEntityDto.IsMainProfile = TestIsMainProfile;

                    LegalPersonProfile.CreatedBy = CurrentCreatedBy;
                    LegalPersonProfile.ModifiedBy = CurrentModifiedBy;
                    LegalPersonProfile.IsActive = !TestIsActive;
                    LegalPersonProfile.IsDeleted = !TestIsDeleted;
                    LegalPersonProfile.CreatedOn = CurrentCreatedOn;
                    LegalPersonProfile.ModifiedOn = CurrentModifiedOn;
                    LegalPersonProfile.IsMainProfile = !TestIsMainProfile;
                };

            private It should_not_change_IsActive_Property = () => Result.IsActive.Should().Be(!TestIsActive);
            private It should_not_change_IsDeleted_Property = () => Result.IsActive.Should().Be(!TestIsDeleted);
            private It should_not_change_CreatedBy_Property = () => Result.CreatedBy.Should().Be(CurrentCreatedBy);
            private It should_not_change_ModifiedBy_Property = () => Result.ModifiedBy.Should().Be(CurrentModifiedBy);
            private It should_not_change_CreatedOn_Property = () => Result.CreatedOn.Should().Be(CurrentCreatedOn);
            private It should_not_change_ModifiedOn_Property = () => Result.ModifiedOn.Should().Be(CurrentModifiedOn);
            private It should_not_change_IsMainProfile_Property = () => Result.IsActive.Should().Be(!TestIsMainProfile);
        }

        [Tags("Obtainer")]
        [Subject(typeof(LegalPersonProfileObtainer))]
        public class When_obtain_entity : LegalPersonProfileObtainerResultContext
        {
            private const string TestName = "TestName";
            private const string TestPositionInNominative = "TestPositionInNominative";
            private const string TestPositionInGenitive = "TestPositionInGenitive";
            private const string TestRegistered = "TestRegistered";
            private const string TestChiefNameInNominative = "TestChiefNameInNominative";
            private const string TestChiefNameInGenitive = "TestChiefNameInGenitive";
            private const OperatesOnTheBasisType TestOperatesOnTheBasisInGenitive = OperatesOnTheBasisType.FoundingBargain;
            private const string TestCertificateNumber = "TestCertificateNumber";
            private const PaymentMethod TestPaymentMethod = PaymentMethod.CreditCard;
            private const string TestAccountNumber = "TestAccountNumber";
            private const string TestBankCode = "TestBankCode";
            private const string TestIBAN = "TestIBAN";
            private const string TestSWIFT = "TestSWIFT";
            private const string TestBankName = "TestBankName";
            private const string TestBankAddress = "TestBankAddress";
            private static readonly DateTime TestCertificateDate = DateTime.UtcNow.AddMinutes(12);
            private const string TestWarrantyNumber = "TestWarrantyNumber";
            private static readonly DateTime TestWarrantyBeginDate = DateTime.UtcNow.AddMinutes(13);
            private static readonly DateTime TestWarrantyEndDate = DateTime.UtcNow.AddMinutes(14);
            private const string TestBargainNumber = "TestBargainNumber";
            private static readonly DateTime TestBargainBeginDate = DateTime.UtcNow.AddMinutes(15);
            private static readonly DateTime TestBargainEndDate = DateTime.UtcNow.AddMinutes(16);
            private const string TestDocumentsDeliveryAddress = "TestDocumentsDeliveryAddress";
            private const string TestPostAddress = "TestPostAddress";
            private const string TestRecipientName = "TestRecipientName";
            private const DocumentsDeliveryMethod TestDocumentsDeliveryMethod = DocumentsDeliveryMethod.DeliveryByClient;
            private const string TestEmailForAccountingDocuments = "TestEmailForAccountingDocuments";
            private const string TestAdditionalEmail = "TestAdditionalEmail";
            private const string TestPersonResponsibleForDocuments = "TestPersonResponsibleForDocuments";
            private const string TestPhone = "TestPhone";
            private const string TestPaymentEssentialElements = "TestPaymentEssentialElements";
            private const string TestRegistrationCertificateNumber = "TestRegistrationCertificateNumber";
            private static readonly DateTime TestRegistrationCertificateDate = DateTime.UtcNow.AddMinutes(17);

            private static readonly byte[] TestTimestamp = { 1, 2, 3, 4, 5, 6, 7, 8 };

            private static readonly EntityReference TestOwner = new EntityReference(234, "TestOwner");
            private static readonly EntityReference TestLegalPerson = new EntityReference(234, "TestTestLegalPerson");

            private Establish context = () =>
                {
                    DomainEntityDto.Name = TestName;
                    DomainEntityDto.PositionInNominative = TestPositionInNominative;
                    DomainEntityDto.PositionInGenitive = TestPositionInGenitive;
                    DomainEntityDto.Registered = TestRegistered;
                    DomainEntityDto.ChiefNameInNominative = TestChiefNameInNominative;
                    DomainEntityDto.ChiefNameInGenitive = TestChiefNameInGenitive;
                    DomainEntityDto.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisInGenitive;
                    DomainEntityDto.CertificateNumber = TestCertificateNumber;
                    DomainEntityDto.PaymentMethod = TestPaymentMethod;
                    DomainEntityDto.AccountNumber = TestAccountNumber;
                    DomainEntityDto.BankCode = TestBankCode;
                    DomainEntityDto.IBAN = TestIBAN;
                    DomainEntityDto.SWIFT = TestSWIFT;
                    DomainEntityDto.BankName = TestBankName;
                    DomainEntityDto.BankAddress = TestBankAddress;
                    DomainEntityDto.CertificateDate = TestCertificateDate;
                    DomainEntityDto.WarrantyNumber = TestWarrantyNumber;
                    DomainEntityDto.WarrantyBeginDate = TestWarrantyBeginDate;
                    DomainEntityDto.WarrantyEndDate = TestWarrantyEndDate;
                    DomainEntityDto.BargainNumber = TestBargainNumber;
                    DomainEntityDto.BargainBeginDate = TestBargainBeginDate;
                    DomainEntityDto.BargainEndDate = TestBargainEndDate;
                    DomainEntityDto.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
                    DomainEntityDto.PostAddress = TestPostAddress;
                    DomainEntityDto.RecipientName = TestRecipientName;
                    DomainEntityDto.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
                    DomainEntityDto.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
                    DomainEntityDto.AdditionalEmail = TestAdditionalEmail;
                    DomainEntityDto.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
                    DomainEntityDto.Phone = TestPhone;
                    DomainEntityDto.PaymentEssentialElements = TestPaymentEssentialElements;
                    DomainEntityDto.RegistrationCertificateNumber = TestRegistrationCertificateNumber;
                    DomainEntityDto.RegistrationCertificateDate = TestRegistrationCertificateDate;
                    DomainEntityDto.LegalPersonRef = TestLegalPerson;
                    DomainEntityDto.Timestamp = TestTimestamp;
                    DomainEntityDto.OwnerRef = TestOwner;

                };

            private It should_return_expected_Name = () => Result.Name.Should().Be(TestName);
            private It should_return_expected_PositionInNominative = () => Result.PositionInNominative.Should().Be(TestPositionInNominative);
            private It should_return_expected_PositionInGenitive = () => Result.PositionInGenitive.Should().Be(TestPositionInGenitive);
            private It should_return_expected_Registered = () => Result.Registered.Should().Be(TestRegistered);
            private It should_return_expected_ChiefNameInNominative = () => Result.ChiefNameInNominative.Should().Be(TestChiefNameInNominative);
            private It should_return_expected_ChiefNameInGenitive = () => Result.ChiefNameInGenitive.Should().Be(TestChiefNameInGenitive);
            private It should_return_expected_OperatesOnTheBasisInGenitive = () => Result.OperatesOnTheBasisInGenitive.Should().Be(TestOperatesOnTheBasisInGenitive);
            private It should_return_expected_CertificateNumber = () => Result.CertificateNumber.Should().Be(TestCertificateNumber);
            private It should_return_expected_PaymentMethod = () => Result.PaymentMethod.Should().Be(TestPaymentMethod);
            private It should_return_expected_AccountNumber = () => Result.AccountNumber.Should().Be(TestAccountNumber);
            private It should_return_expected_BankCode = () => Result.BankCode.Should().Be(TestBankCode);
            private It should_return_expected_IBAN = () => Result.IBAN.Should().Be(TestIBAN);
            private It should_return_expected_SWIFT = () => Result.SWIFT.Should().Be(TestSWIFT);
            private It should_return_expected_BankName = () => Result.BankName.Should().Be(TestBankName);
            private It should_return_expected_BankAddress = () => Result.BankAddress.Should().Be(TestBankAddress);
            private It should_return_expected_CertificateDate = () => Result.CertificateDate.Should().Be(TestCertificateDate);
            private It should_return_expected_WarrantyNumber = () => Result.WarrantyNumber.Should().Be(TestWarrantyNumber);
            private It should_return_expected_WarrantyBeginDate = () => Result.WarrantyBeginDate.Should().Be(TestWarrantyBeginDate);
            private It should_return_expected_WarrantyEndDate = () => Result.WarrantyEndDate.Should().Be(TestWarrantyEndDate);
            private It should_return_expected_BargainNumber = () => Result.BargainNumber.Should().Be(TestBargainNumber);
            private It should_return_expected_BargainBeginDate = () => Result.BargainBeginDate.Should().Be(TestBargainBeginDate);
            private It should_return_expected_BargainEndDate = () => Result.BargainEndDate.Should().Be(TestBargainEndDate);
            private It should_return_expected_DocumentsDeliveryAddress = () => Result.DocumentsDeliveryAddress.Should().Be(TestDocumentsDeliveryAddress);
            private It should_return_expected_PostAddress = () => Result.PostAddress.Should().Be(TestPostAddress);
            private It should_return_expected_RecipientName = () => Result.RecipientName.Should().Be(TestRecipientName);
            private It should_return_expected_DocumentsDeliveryMethod = () => Result.DocumentsDeliveryMethod.Should().Be(TestDocumentsDeliveryMethod);
            private It should_return_expected_EmailForAccountingDocuments = () => Result.EmailForAccountingDocuments.Should().Be(TestEmailForAccountingDocuments);
            private It should_return_expected_AdditionalEmail = () => Result.AdditionalEmail.Should().Be(TestAdditionalEmail);
            private It should_return_expected_PersonResponsibleForDocuments = () => Result.PersonResponsibleForDocuments.Should().Be(TestPersonResponsibleForDocuments);
            private It should_return_expected_Phone = () => Result.Phone.Should().Be(TestPhone);
            private It should_return_expected_PaymentEssentialElements = () => Result.PaymentEssentialElements.Should().Be(TestPaymentEssentialElements);
            private It should_return_expected_RegistrationCertificateNumber = () => Result.RegistrationCertificateNumber.Should().Be(TestRegistrationCertificateNumber);
            private It should_return_expected_RegistrationCertificateDate = () => Result.RegistrationCertificateDate.Should().Be(TestRegistrationCertificateDate);
            private It should_return_expected_Timestamp = () => Result.Timestamp.Should().BeEquivalentTo(TestTimestamp);
            private It should_return_expected_Owner = () => Result.OwnerCode.Should().Be(TestOwner.Id);
            private It should_return_expected_LegalPerson = () => Result.OwnerCode.Should().Be(TestLegalPerson.Id);

        }

        private static IQueryable<T> Q<T>(params T[] parameters)
        {
            return parameters.AsQueryable();
        }
    }
}