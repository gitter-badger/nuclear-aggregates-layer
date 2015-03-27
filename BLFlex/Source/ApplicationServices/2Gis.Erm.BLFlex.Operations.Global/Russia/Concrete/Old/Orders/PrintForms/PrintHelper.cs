using System;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public static class PrintHelper
    {
        public static PrintData DetermineRequisitesType(LegalPersonType legalPersonType)
        {
            return new PrintData
                       {
                           { "UseLegalPerson", legalPersonType == LegalPersonType.LegalPerson },
                           { "UseBusinessman", legalPersonType == LegalPersonType.Businessman },
                           { "UseNaturalPerson", legalPersonType == LegalPersonType.NaturalPerson },
                       };
        }

        public static PrintData LegalPersonRequisites(LegalPerson legalPerson)
        {
            return new PrintData
                       {
                           {
                               "LegalPerson", new PrintData
                                                  {
                                                      { "Inn", legalPerson.Inn },
                                                      { "Kpp", legalPerson.Kpp },
                                                      { "LegalAddress", legalPerson.LegalAddress },
                                                      { "LegalName", legalPerson.LegalName },
                                                      { "PassportSeries", legalPerson.PassportSeries },
                                                      { "PassportNumber", legalPerson.PassportNumber },
                                                      { "PassportIssuedBy", legalPerson.PassportIssuedBy },
                                                      { "RegistrationAddress", legalPerson.RegistrationAddress },
                                                      { "ShortName", legalPerson.ShortName },
                                                  }
                           },
                       };
        }

        public static PrintData LegalPersonProfileRequisites(LegalPersonProfile legalPersonProfile)
        {
            return new PrintData
                       {
                           {
                               "LegalPersonProfile", new PrintData
                                                         {
                                                             { "Phone", legalPersonProfile.Phone },
                                                             { "PaymentEssentialElements", legalPersonProfile.PaymentEssentialElements },
                                                             { "PositionInNominative", legalPersonProfile.PositionInNominative },
                                                             { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                                                         }
                           },
                       };
        }

        public static PrintData BranchOfficeRequisites(BranchOffice branchOffice)
        {
            return new PrintData
                       {
                           {
                               "BranchOffice", new PrintData
                                                   {
                                                       { "Inn", branchOffice.Inn },
                                                       { "LegalAddress", branchOffice.LegalAddress },
                                                   }
                           },
                       };
        }

        public static PrintData BranchOfficeOrganizationUnitRequisites(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                       {
                           {
                               "BranchOfficeOrganizationUnit", new PrintData
                                                                   {
                                                                       { "ShortLegalName", branchOfficeOrganizationUnit.ShortLegalName },
                                                                       { "PhoneNumber", branchOfficeOrganizationUnit.PhoneNumber },
                                                                       { "PaymentEssentialElements", branchOfficeOrganizationUnit.PaymentEssentialElements },
                                                                       { "PositionInNominative", branchOfficeOrganizationUnit.PositionInNominative },
                                                                       { "ChiefNameInNominative", branchOfficeOrganizationUnit.ChiefNameInNominative },
                                                                       { "ActualAddress", branchOfficeOrganizationUnit.ActualAddress },
                                                                       { "Kpp", branchOfficeOrganizationUnit.Kpp },
                                                                       { "Email", branchOfficeOrganizationUnit.Email },
                                                                   }
                           },
                       };
        }

        public static PrintData DetermineBilletType(ContributionTypeEnum contributionType)
        {
            return new PrintData
                       {
                           { "UseWithRequisitesWarning", contributionType == ContributionTypeEnum.Branch },
                           { "UseWithoutRequisitesWarning", contributionType != ContributionTypeEnum.Branch },
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
