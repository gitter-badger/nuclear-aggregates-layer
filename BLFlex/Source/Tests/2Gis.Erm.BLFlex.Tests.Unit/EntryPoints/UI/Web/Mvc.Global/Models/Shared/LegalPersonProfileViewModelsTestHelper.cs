using System;

using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Models.Shared
{
    public static class LegalPersonViewProfileModelsTestHelper
    {
        public const string TestName = "TestName";
        public static readonly EntityReference TestLegalPerson = new EntityReference(2, "TestLegalPerson");
        public const string TestDocumentsDeliveryAddress = "Test DocumentsDeliveryAddress";
        public const string TestRecipientName = "TestRecipientName";
        public const DocumentsDeliveryMethod TestDocumentsDeliveryMethod = DocumentsDeliveryMethod.DeliveryByManager;
        public const string TestPersonResponsibleForDocuments = "TestPersonResponsibleForDocuments";
        public const string TestEmailForAccountingDocuments = "TestEmailForAccountingDocuments";
        public const string TestAdditionalEmail = "TestAdditionalEmail";
        public const string TestPostAddress = "PostAddress";
        public const PaymentMethod TestPaymentMethod = PaymentMethod.CreditCard;
        public const string TestAccountNumber = "TestAccountNumber";
        public const string TestMfo = "TestMfo";
        public const string TestBankName = "TestBankName";
        public const string TestPhone = "TestPhone";
        public const OperatesOnTheBasisType TestOperatesOnTheBasisType = OperatesOnTheBasisType.FoundingBargain;
        public const string TestPositionInGenitive = "TestPositionInGenitive";
        public const string TestPositionInNominative = "TestPositionInNominative";
        public const string TestChiefNameInNominative = "TestChiefNameInNominative";
        public const string TestChiefNameInGenitive = "TestChiefNameInGenitive";
        public const string TestCertificateNumber = "TestCertificateNumber";
        public static readonly DateTime TestCertificateDate = DateTime.UtcNow.AddMinutes(3);
        public const string TestWarrantyNumber = "TestWarrantyNumber";
        public static readonly DateTime TestWarrantyBeginDate = DateTime.UtcNow.AddMinutes(5);
        public static readonly DateTime TestWarrantyEndDate = DateTime.UtcNow.AddMinutes(7);
        public const long TestId = 5;
        public static readonly byte[] TestTimestamp = { 1, 2, 3, 4, 5, 6, 7, 8 };
        public const LegalPersonType TestLegalPersonType = LegalPersonType.Businessman;
        public const bool TestIsMainProfile = true;
        public const AccountType TestAccountType = AccountType.SavingsAccount;
        public static readonly EntityReference TestBank = new EntityReference(123, "TestBank");
        public const string TestRepresentativeRut = "TestRepresentativeRut";
        public static readonly DateTime TestRepresentativeDocumentIssuedOn = DateTime.UtcNow.AddMinutes(9);
        public const string TestRepresentativeDocumentIssuedBy = "TestRepresentativeDocumentIssuedBy";
        public const string TestIBAN = "TestIBAN";
        public const string TestSWIFT = "TestSWIFT";
        public const string TestRegistrationCertificateNumber = "TestRegistrationCertificateNumber";
        public static readonly DateTime TestRegistrationCertificateDate = DateTime.UtcNow.AddMinutes(12);
        public const string TestBargainNumber = "TestBargainNumber";
        public static readonly DateTime TestBargainBeginDate = DateTime.UtcNow.AddMinutes(15);
        public static readonly DateTime TestBargainEndDate = DateTime.UtcNow.AddMinutes(17);
        public const string TestPaymentEssentialElements = "TestPaymentEssentialElements";
        public const string TestRegistered = "TestRegistered";
        public const string TestBankCode = "TestBankCode";
        public const string TestBankAddress = "TestBankAddress";

        public static void FillUkraineLegalPersonProfileDtoWithTestData(UkraineLegalPersonProfileDomainEntityDto entityDto)
        {
            entityDto.Id = TestId;
            entityDto.Name = TestName;
            entityDto.LegalPersonRef = TestLegalPerson;
            entityDto.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            entityDto.RecipientName = TestRecipientName;
            entityDto.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            entityDto.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            entityDto.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            entityDto.Email = TestAdditionalEmail;
            entityDto.PostAddress = TestPostAddress;
            entityDto.PaymentMethod = TestPaymentMethod;
            entityDto.AccountNumber = TestAccountNumber;
            entityDto.BankName = TestBankName;
            entityDto.Phone = TestPhone;
            entityDto.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            entityDto.PositionInGenitive = TestPositionInGenitive;
            entityDto.PositionInNominative = TestPositionInNominative;
            entityDto.ChiefNameInNominative = TestChiefNameInNominative;
            entityDto.ChiefNameInGenitive = TestChiefNameInGenitive;
            entityDto.CertificateNumber = TestCertificateNumber;
            entityDto.CertificateDate = TestCertificateDate;
            entityDto.WarrantyNumber = TestWarrantyNumber;
            entityDto.WarrantyBeginDate = TestWarrantyBeginDate;
            entityDto.WarrantyEndDate = TestWarrantyEndDate;
            entityDto.Timestamp = TestTimestamp;
            entityDto.LegalPersonType = TestLegalPersonType;
            entityDto.IsMainProfile = TestIsMainProfile;
            entityDto.PaymentEssentialElements = TestPaymentEssentialElements;
            entityDto.Mfo = TestMfo;    
        }

        public static void FillChileLegalPersonProfileDtoWithTestData(ChileLegalPersonProfileDomainEntityDto entityDto)
        {
            entityDto.Id = TestId;
            entityDto.Name = TestName;
            entityDto.LegalPersonRef = TestLegalPerson;
            entityDto.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            entityDto.RecipientName = TestRecipientName;
            entityDto.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            entityDto.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            entityDto.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            entityDto.Email = TestAdditionalEmail;
            entityDto.PostAddress = TestPostAddress;
            entityDto.PaymentMethod = TestPaymentMethod;
            entityDto.AccountNumber = TestAccountNumber;
            entityDto.Phone = TestPhone;
            entityDto.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            entityDto.PositionInGenitive = TestPositionInGenitive;
            entityDto.PositionInNominative = TestPositionInNominative;
            entityDto.ChiefNameInNominative = TestChiefNameInNominative;
            entityDto.ChiefNameInGenitive = TestChiefNameInGenitive;
            entityDto.Timestamp = TestTimestamp;
            entityDto.IsMainProfile = TestIsMainProfile;
            entityDto.AccountType = TestAccountType;
            entityDto.BankRef = TestBank;
            entityDto.RepresentativeRut = TestRepresentativeRut;
            entityDto.RepresentativeDocumentIssuedOn = TestRepresentativeDocumentIssuedOn;
            entityDto.RepresentativeDocumentIssuedBy = TestRepresentativeDocumentIssuedBy;
        }

        public static LegalPersonProfileDomainEntityDto FillCyprusLegalPersonProfileDtoWithTestData(LegalPersonProfileDomainEntityDto entityDto)
        {
            FillEntityDtoWithData(entityDto);

            return entityDto;
        }

        public static LegalPersonProfileDomainEntityDto FillRussiaLegalPersonProfileDtoWithTestData(LegalPersonProfileDomainEntityDto entityDto)
        {
            FillEntityDtoWithData(entityDto);

            return entityDto;
        }

        public static LegalPersonProfileDomainEntityDto FillCzechLegalPersonProfileDtoWithTestData(LegalPersonProfileDomainEntityDto entityDto)
        {
            FillEntityDtoWithData(entityDto);

            return entityDto;
        }

        public static void FillUkraineViewModelWithTestData(UkraineLegalPersonProfileViewModel model)
        {
            model.Id = TestId;
            model.Name = TestName;
            model.LegalPerson = TestLegalPerson.ToLookupField();
            model.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            model.RecipientName = TestRecipientName;
            model.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            model.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            model.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            model.Email = TestAdditionalEmail;
            model.PostAddress = TestPostAddress;
            model.PaymentMethod = TestPaymentMethod;
            model.AccountNumber = TestAccountNumber;
            model.Mfo = TestMfo;
            model.BankName = TestBankName;
            model.Phone = TestPhone;
            model.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            model.PositionInGenitive = TestPositionInGenitive;
            model.PositionInNominative = TestPositionInNominative;
            model.ChiefNameInNominative = TestChiefNameInNominative;
            model.ChiefNameInGenitive = TestChiefNameInGenitive;
            model.CertificateNumber = TestCertificateNumber;
            model.CertificateDate = TestCertificateDate;
            model.WarrantyNumber = TestWarrantyNumber;
            model.WarrantyBeginDate = TestWarrantyBeginDate;
            model.WarrantyEndDate = TestWarrantyEndDate;
            model.Timestamp = TestTimestamp;
        }

        public static void FillChileViewModelWithTestData(ChileLegalPersonProfileViewModel model)
        {
            model.Id = TestId;
            model.Name = TestName;
            model.LegalPerson = TestLegalPerson.ToLookupField();
            model.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            model.RecipientName = TestRecipientName;
            model.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            model.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            model.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            model.Email = TestAdditionalEmail;
            model.PostAddress = TestPostAddress;
            model.PaymentMethod = TestPaymentMethod;
            model.BankAccountType = TestAccountType;
            model.AccountNumber = TestAccountNumber;
            model.Phone = TestPhone;
            model.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            model.Timestamp = TestTimestamp;
            model.RepresentativeName = TestChiefNameInNominative;
            model.RepresentativePosition = TestPositionInNominative;
            model.RepresentativeRut = TestRepresentativeRut;
            model.RepresentativeDocumentIssuedOn = TestRepresentativeDocumentIssuedOn;
            model.RepresentativeDocumentIssuedBy = TestRepresentativeDocumentIssuedBy;
            model.Bank = TestBank.ToLookupField();
        }

        public static void FillCyprusViewModelWithTestData(CyprusLegalPersonProfileViewModel model)
        {
            model.Id = TestId;
            model.Name = TestName;
            model.LegalPerson = TestLegalPerson.ToLookupField();
            model.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            model.RecipientName = TestRecipientName;
            model.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            model.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            model.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            model.Email = TestAdditionalEmail;
            model.PostAddress = TestPostAddress;
            model.PaymentMethod = TestPaymentMethod;
            model.AccountNumber = TestAccountNumber;
            model.Phone = TestPhone;
            model.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            model.Timestamp = TestTimestamp;
            model.IBAN = TestIBAN;
            model.PositionInGenitive = TestPositionInGenitive;
            model.PositionInNominative = TestPositionInNominative;
            model.RegistrationCertificateDate = TestRegistrationCertificateDate;
            model.RegistrationCertificateNumber = TestRegistrationCertificateNumber;
            model.SWIFT = TestSWIFT;
            model.WarrantyBeginDate = TestWarrantyBeginDate;
            model.WarrantyEndDate = TestWarrantyEndDate;
            model.WarrantyNumber = TestWarrantyNumber;
            model.BankName = TestBankName;
            model.BargainNumber = TestBargainNumber;
            model.BargainBeginDate = TestBargainBeginDate;
            model.CertificateDate = TestCertificateDate;
            model.ChiefNameInGenitive = TestChiefNameInGenitive;
            model.CertificateNumber = TestCertificateNumber;
            model.ChiefNameInNominative = TestChiefNameInNominative;
            model.BargainEndDate = TestBargainEndDate;
        }

        public static void FillRussiaViewModelWithTestData(LegalPersonProfileViewModel model)
        {
            model.Id = TestId;
            model.Name = TestName;
            model.LegalPerson = TestLegalPerson.ToLookupField();
            model.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            model.RecipientName = TestRecipientName;
            model.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            model.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            model.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            model.Email = TestAdditionalEmail;
            model.PostAddress = TestPostAddress;
            model.Phone = TestPhone;
            model.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            model.Timestamp = TestTimestamp;
            model.PositionInGenitive = TestPositionInGenitive;
            model.PositionInNominative = TestPositionInNominative;
            model.WarrantyBeginDate = TestWarrantyBeginDate;
            model.WarrantyEndDate = TestWarrantyEndDate;
            model.WarrantyNumber = TestWarrantyNumber;
            model.BargainNumber = TestBargainNumber;
            model.BargainBeginDate = TestBargainBeginDate;
            model.CertificateDate = TestCertificateDate;
            model.ChiefNameInGenitive = TestChiefNameInGenitive;
            model.CertificateNumber = TestCertificateNumber;
            model.ChiefNameInNominative = TestChiefNameInNominative;
            model.BargainEndDate = TestBargainEndDate;
            model.PaymentEssentialElements = TestPaymentEssentialElements;
        }

        public static void FillCzechViewModelWithTestData(CzechLegalPersonProfileViewModel model)
        {
            model.Id = TestId;
            model.Name = TestName;
            model.LegalPerson = TestLegalPerson.ToLookupField();
            model.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            model.RecipientName = TestRecipientName;
            model.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            model.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            model.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            model.Email = TestAdditionalEmail;
            model.PostAddress = TestPostAddress;
            model.PaymentMethod = TestPaymentMethod;
            model.AccountNumber = TestAccountNumber;
            model.Phone = TestPhone;
            model.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            model.Timestamp = TestTimestamp;
            model.PositionInGenitive = TestPositionInGenitive;
            model.PositionInNominative = TestPositionInNominative;
            model.WarrantyBeginDate = TestWarrantyBeginDate;
            model.BankName = TestBankName;
            model.ChiefNameInGenitive = TestChiefNameInGenitive;
            model.ChiefNameInNominative = TestChiefNameInNominative;
            model.Registered = TestRegistered;
            model.BankCode = TestBankCode;
            model.BankAddress = TestBankAddress;
        }

        private static void FillEntityDtoWithData(LegalPersonProfileDomainEntityDto entityDto)
        {
            entityDto.Id = TestId;
            entityDto.Name = TestName;
            entityDto.LegalPersonRef = TestLegalPerson;
            entityDto.DocumentsDeliveryAddress = TestDocumentsDeliveryAddress;
            entityDto.RecipientName = TestRecipientName;
            entityDto.DocumentsDeliveryMethod = TestDocumentsDeliveryMethod;
            entityDto.PersonResponsibleForDocuments = TestPersonResponsibleForDocuments;
            entityDto.EmailForAccountingDocuments = TestEmailForAccountingDocuments;
            entityDto.Email = TestAdditionalEmail;
            entityDto.PostAddress = TestPostAddress;
            entityDto.PaymentMethod = TestPaymentMethod;
            entityDto.AccountNumber = TestAccountNumber;
            entityDto.BankName = TestBankName;
            entityDto.Phone = TestPhone;
            entityDto.OperatesOnTheBasisInGenitive = TestOperatesOnTheBasisType;
            entityDto.PositionInGenitive = TestPositionInGenitive;
            entityDto.PositionInNominative = TestPositionInNominative;
            entityDto.ChiefNameInNominative = TestChiefNameInNominative;
            entityDto.ChiefNameInGenitive = TestChiefNameInGenitive;
            entityDto.CertificateNumber = TestCertificateNumber;
            entityDto.CertificateDate = TestCertificateDate;
            entityDto.WarrantyNumber = TestWarrantyNumber;
            entityDto.WarrantyBeginDate = TestWarrantyBeginDate;
            entityDto.WarrantyEndDate = TestWarrantyEndDate;
            entityDto.Timestamp = TestTimestamp;
            entityDto.LegalPersonType = TestLegalPersonType;
            entityDto.IsMainProfile = TestIsMainProfile;
            entityDto.IBAN = TestIBAN;
            entityDto.SWIFT = TestSWIFT;
            entityDto.RegistrationCertificateNumber = TestRegistrationCertificateNumber;
            entityDto.RegistrationCertificateDate = TestRegistrationCertificateDate;
            entityDto.BargainNumber = TestBargainNumber;
            entityDto.BargainBeginDate = TestBargainBeginDate;
            entityDto.BargainEndDate = TestBargainEndDate;
            entityDto.PaymentEssentialElements = TestPaymentEssentialElements;
            entityDto.Registered = TestRegistered;
            entityDto.BankCode = TestBankCode;
            entityDto.BankAddress = TestBankAddress;
        }
    }
}
