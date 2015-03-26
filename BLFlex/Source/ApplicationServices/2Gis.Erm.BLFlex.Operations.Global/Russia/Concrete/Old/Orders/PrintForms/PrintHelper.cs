using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public static class PrintHelper
    {
        public static PrintData Requisites(LegalPerson legalPerson, LegalPersonProfile legalPersonProfile, BranchOffice branchOffice, BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                       {
                           { "UseLegalPerson", legalPerson.LegalPersonTypeEnum == LegalPersonType.LegalPerson },
                           { "UseBusinessman", legalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman },
                           { "UseNaturalPerson", legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson },

                           { "BranchOffice.Inn", branchOffice.Inn },
                           { "BranchOffice.LegalAddress", branchOffice.LegalAddress },
                           { "BranchOfficeOrganizationUnit.ShortLegalName", branchOfficeOrganizationUnit.ShortLegalName },
                           { "BranchOfficeOrganizationUnit.Kpp", branchOfficeOrganizationUnit.Kpp },
                           { "BranchOfficeOrganizationUnit.ActualAddress", branchOfficeOrganizationUnit.ActualAddress },
                           { "BranchOfficeOrganizationUnit.PaymentEssentialElements", branchOfficeOrganizationUnit.PaymentEssentialElements },
                           { "BranchOfficeOrganizationUnit.Email", branchOfficeOrganizationUnit.Email },
                           { "BranchOfficeOrganizationUnit.PositionInNominative", branchOfficeOrganizationUnit.PositionInNominative },
                           { "BranchOfficeOrganizationUnit.ChiefNameInNominative", branchOfficeOrganizationUnit.ChiefNameInNominative },

                           { "LegalPerson.Inn", legalPerson.Inn },
                           { "LegalPerson.Kpp", legalPerson.Kpp },
                           { "LegalPerson.LegalAddress", legalPerson.LegalAddress },
                           { "LegalPerson.LegalName", legalPerson.LegalName },
                           { "LegalPerson.PassportSeries", legalPerson.PassportSeries },
                           { "LegalPerson.PassportNumber", legalPerson.PassportNumber },
                           { "LegalPerson.PassportIssuedBy", legalPerson.PassportIssuedBy },
                           { "LegalPerson.RegistrationAddress", legalPerson.RegistrationAddress },
                           { "LegalPerson.ShortName", legalPerson.ShortName },
                           { "LegalPersonProfile.PaymentEssentialElements", legalPersonProfile.PaymentEssentialElements },
                           { "LegalPersonProfile.PositionInNominative", legalPersonProfile.PositionInNominative },
                           { "LegalPersonProfile.ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                       };
        }

        public static PrintData RelatedBrgain(Bargain bargain)
        {
            return bargain == null
                       ? new PrintData
                             {
                                 { "UseBargain", false },
                             }
                       : new PrintData
                             {
                                 { "UseBargain", true },
                                 { "Bargain.Number", bargain.Number },
                                 { "Bargain.CreatedOn", bargain.CreatedOn }
                             };
        }

        public static PrintData AgreementSharedBody(
            Order order,
            LegalPerson legalPerson,
            LegalPersonProfile legalPersonProfile,
            BranchOfficeOrganizationUnit branchOfficeOrganizationUnit,
            IFormatter dateFormatter)
        {
            return new PrintData
                       {
                           // Эти поля используются во всех доп соглашениях:
                           { "Order.Number", order.Number },
                           { "Order.CreatedOn", order.CreatedOn },
                           { "LegalPerson.LegalName", legalPerson.LegalName },

                           { "LegalPersonProfile.PositionInGenitive", legalPersonProfile.PositionInGenitive },
                           { "LegalPersonProfile.ChiefNameInGenitive", legalPersonProfile.ChiefNameInGenitive },

                           { "BranchOfficeOrganizationUnit.ShortLegalName", branchOfficeOrganizationUnit.ShortLegalName },
                           { "BranchOfficeOrganizationUnit.ApplicationCityName", branchOfficeOrganizationUnit.ApplicationCityName },
                           { "BranchOfficeOrganizationUnit.PositionInGenitive", branchOfficeOrganizationUnit.PositionInGenitive },
                           { "BranchOfficeOrganizationUnit.ChiefNameInGenitive", branchOfficeOrganizationUnit.ChiefNameInGenitive },
                           { "BranchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive", branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive },

                           { "OperatesOnTheBasisInGenitive", GetOperatesOnTheBasisInGenitive(legalPersonProfile, legalPerson.LegalPersonTypeEnum, dateFormatter) },
                       };
        }

        public static PrintData TerminationAgreementSpecificBody(Order order)
        {
            return new PrintData
                       {
                           { "NextReleaseDate", order.RejectionDate.HasValue ? order.RejectionDate.Value.GetNextMonthFirstDate() : default(DateTime) },
                           { "Order.RejectionDate", order.RejectionDate.HasValue ? order.RejectionDate.Value : default(DateTime) },
                       };
        }

        public static PrintData ChangeAgreementSpecificBody(Firm firm)
        {
            return new PrintData
                       {
                           { "DateToday", DateTime.Today },
                           { "Firm.Name", firm.Name },
                       };
        }

        private static string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile, LegalPersonType legalPersonType, IFormatter dateFormatter)
        {
            switch (profile.OperatesOnTheBasisInGenitive)
            {
                case null:
                    return string.Empty;
                case OperatesOnTheBasisType.Undefined:
                    return string.Empty;
                case OperatesOnTheBasisType.Charter:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCharterTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                case OperatesOnTheBasisType.Certificate:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCertificateTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.CertificateNumber,
                        dateFormatter.Format(profile.CertificateDate));
                case OperatesOnTheBasisType.Warranty:
                    return string.Format(
                        legalPersonType == LegalPersonType.NaturalPerson
                            ? BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson
                            : BLResources.OperatesOnBasisOfWarantyTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.WarrantyNumber,
                        dateFormatter.Format(profile.WarrantyBeginDate));
                case OperatesOnTheBasisType.Bargain:
                    return string.Format(
                        BLResources.OperatesOnBasisOfBargainTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.BargainNumber,
                        dateFormatter.Format(profile.BargainBeginDate));
                case OperatesOnTheBasisType.FoundingBargain:
                    return string.Format(
                        BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
