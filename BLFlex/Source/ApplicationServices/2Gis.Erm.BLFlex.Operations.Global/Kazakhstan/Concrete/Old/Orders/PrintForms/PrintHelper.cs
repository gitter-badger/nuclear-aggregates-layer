using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    public class PrintHelper
    {
        public static PrintData BargainFlagFields(Bargain bargain)
        {
            return new PrintData
                {
                    { "UseLimitedBargain", bargain.BargainEndDate != null },
                    { "UseEndlessBargain", bargain.BargainEndDate == null },
                };
        }

        public static PrintData LegalPersonFlagFields(LegalPerson legalPerson)
        {
            return new PrintData
                {
                    { "UseLegalPerson", legalPerson.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson || legalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman },
                    { "UseNaturalPerson", legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson },
                };
        }

        public static PrintData BargainFields(Bargain bargain)
        {
            return new PrintData
                       {
                           { "Number", bargain.Number },
                           { "SignedOn", bargain.SignedOn },
                           { "EndDate", bargain.BargainEndDate },
                       };
        }

        public static PrintData BranchOfficeFields(BranchOffice branchOffice)
        {
            return new PrintData
                       {
                           { "Inn", branchOffice.Inn },
                           { "LegalAddress", branchOffice.LegalAddress },
                           { "Name", branchOffice.Name },
                       };
        }

        public static PrintData BranchOfficeOrganizationUnitFields(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                       {
                           { "ChiefNameInGenitive", branchOfficeOrganizationUnit.ChiefNameInGenitive },
                           { "ChiefNameInNominative", branchOfficeOrganizationUnit.ChiefNameInNominative },
                           { "PaymentEssentialElements", branchOfficeOrganizationUnit.PaymentEssentialElements },
                           { "PositionInGenitive", branchOfficeOrganizationUnit.PositionInGenitive },
                           { "PositionInNominative", branchOfficeOrganizationUnit.PositionInNominative },
                           { "ShortLegalName", branchOfficeOrganizationUnit.ShortLegalName },
                           { "OperatesOnTheBasisInGenitive", branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive },
                           { "ActualAddress", branchOfficeOrganizationUnit.ActualAddress },
                           { "PhoneNumber", branchOfficeOrganizationUnit.PhoneNumber },
                       };
        }

        public static PrintData LegalPersonFields(LegalPerson legalPerson)
        {
            return new PrintData
                       {
                           { "Inn", legalPerson.Inn },
                           { "LegalAddress", legalPerson.LegalAddress },
                           { "LegalName", legalPerson.LegalName },
                           { "PersonalId", legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(part => part.IdentityCardNumber) },
                           { "PersonalIdIssuedOn", legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(part => part.IdentityCardIssuedOn) },
                           { "PersonalIdIssuedBy", legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(part => part.IdentityCardIssuedBy) },
                       };
        }

        public static PrintData LegalPersonProfileFields(LegalPersonProfile profile)
        {
            return new PrintData
                       {
                           { "LegalPersonProfile.PositionInNominative", profile.PositionInNominative },
                           { "LegalPersonProfile.PositionInGenitive", profile.PositionInGenitive },
                           { "ChiefNameInGenitive", profile.ChiefNameInGenitive },
                           { "ChiefNameInNominative", profile.ChiefNameInNominative },
                           { "PositionInNominative", profile.PositionInNominative },
                           { "PositionInGenitive", profile.PositionInGenitive },
                           { "EmailForAccountingDocuments", profile.EmailForAccountingDocuments },
                           { "BankName", profile.BankName },
                           { "PaymentEssentialElements", profile.PaymentEssentialElements },
                           { "IBAN", profile.IBAN },
                           { "SWIFT", profile.SWIFT },
                           { "ActualAddress", profile.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(part => part.ActualAddress) },
                           { "Phone", profile.Phone },
                       };
        }

        public static PrintData GetAuthorityDocumentDescription(LegalPersonProfile profile, CultureInfo targetCulture)
        {
            if (profile.OperatesOnTheBasisInGenitive == null)
            {
                return new PrintData();
            }

            switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
            {
                case OperatesOnTheBasisType.Undefined:
                    return new PrintData();

                case OperatesOnTheBasisType.Charter:
                    return new PrintData
                               {
                                   { "UseCharterOrOther", true },
                                   { "Name", OperatesOnTheBasisType.Charter.ToStringLocalized(EnumResources.ResourceManager, targetCulture) },
                               };

                case OperatesOnTheBasisType.Certificate:
                    return new PrintData
                               {
                                   { "UseCommon", true },
                                   { "Name", OperatesOnTheBasisType.Certificate.ToStringLocalized(EnumResources.ResourceManager, targetCulture) },
                                   { "Date", profile.CertificateDate },
                                   { "Number", profile.CertificateNumber },
                               };

                case OperatesOnTheBasisType.Warranty:
                    return new PrintData
                               {
                                   { "UseCommon", true },
                                   { "Name", OperatesOnTheBasisType.Warranty.ToStringLocalized(EnumResources.ResourceManager, targetCulture) },
                                   { "Date", profile.WarrantyBeginDate },
                                   { "Number", profile.WarrantyNumber },
                               };

                case OperatesOnTheBasisType.Decree:
                    return new PrintData
                               {
                                   { "UseCommon", true },
                                   { "Name", OperatesOnTheBasisType.Decree.ToStringLocalized(EnumResources.ResourceManager, targetCulture) },
                                   { "Date", profile.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(x => x.DecreeDate) },
                                   { "Number", profile.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(x => x.DecreeNumber) },
                               };

                case OperatesOnTheBasisType.Other:
                    return new PrintData
                               {
                                   { "UseCharterOrOther", true },
                                   { "Name", profile.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(x => x.OtherAuthorityDocument) },
                               };

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static PrintData OrderFields(Order order)
        {
            return new PrintData
                       {
                           { "Number", order.Number },
                           { "SignupDate", order.SignupDate },
                           { "RejectionDate", order.RejectionDate }
                       };
        }
    }
}