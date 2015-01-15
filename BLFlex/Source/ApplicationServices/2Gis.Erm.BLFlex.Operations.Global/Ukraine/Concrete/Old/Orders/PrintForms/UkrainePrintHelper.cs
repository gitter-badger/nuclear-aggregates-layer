using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.PrintForms
{
    public class UkrainePrintHelper
    {
        private readonly IFormatter _shortDateFormatter;

        public UkrainePrintHelper(IFormatterFactory formatterFactory)
        {
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }


        public static PrintData BranchOfficeFields(BranchOffice branchOffice)
        {
            return new PrintData
                {
                    { "Ipn", branchOffice.Within<UkraineBranchOfficePart>().GetPropertyValue(part => part.Ipn) },
                    { "Egrpou", branchOffice.Inn },
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
                    { "PhoneNumber", branchOfficeOrganizationUnit.PhoneNumber },
                };
        }

        public static PrintData OrderFields(Order order)
        {
            return new PrintData
                {
                    { "SignupDate", order.SignupDate },
                    { "CreatedOn", order.CreatedOn },
                    { "RejectionDate", order.RejectionDate },
                    { "Number", order.Number },
                    { "BeginDistributionDate", order.BeginDistributionDate },
                    { "EndDistributionDate", order.EndDistributionDatePlan },
                };
        }

        public static PrintData LegalPersonFields(LegalPerson legalPerson)
        {
            return new PrintData
                {
                    { "Egrpou", legalPerson.Within<UkraineLegalPersonPart>().GetPropertyValue(part => part.Egrpou) },
                    { "Ipn", legalPerson.Inn },
                    { "LegalAddress", legalPerson.LegalAddress },
                    { "LegalName", legalPerson.LegalName },
                };
        }

        public static PrintData LegalPersonProfileFields(LegalPersonProfile profile)
        {
            return new PrintData
                {
                    { "ChiefNameInGenitive", profile.ChiefNameInGenitive },
                    { "ChiefNameInNominative", profile.ChiefNameInNominative },
                    { "PositionInGenitive", profile.PositionInGenitive },
                    { "PositionInNominative", profile.PositionInNominative },
                    { "EmailForAccountingDocuments", profile.EmailForAccountingDocuments },
                    { "Phone", profile.Phone },
                    { "BankName", profile.BankName },
                    { "AccountNumber", profile.AccountNumber },
                           {
                               "PaymentMethod",
                               profile.PaymentMethod != PaymentMethod.Undefined
                                   ? profile.PaymentMethod.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                                   : string.Empty
                           },
                    { "PaymentEssentialElements", profile.PaymentEssentialElements },
                    { "Mfo", profile.Within<UkraineLegalPersonProfilePart>().GetPropertyValue(part => part.Mfo) },
                };
        }

        public string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile)
        {
            if (profile == null || profile.OperatesOnTheBasisInGenitive == null)
            {
                return string.Empty;
            }

            switch (profile.OperatesOnTheBasisInGenitive)
            {
                case OperatesOnTheBasisType.Charter:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCharterTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                case OperatesOnTheBasisType.Certificate:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCertificateTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.CertificateNumber,
                        _shortDateFormatter.Format(profile.CertificateDate.Value));
                case OperatesOnTheBasisType.Warranty:
                    return string.Format(
                        BLResources.OperatesOnBasisOfWarantyTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.WarrantyNumber,
                        _shortDateFormatter.Format(profile.WarrantyBeginDate.Value));
                case OperatesOnTheBasisType.FoundingBargain:
                    return string.Format(
                        BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                default:
                    return string.Empty;
            }
        }

        public string GetRelatedBargainInfo(Bargain bargain)
        {
            if (bargain == null)
            {
                return string.Empty;
            }

            return string.Format(BLResources.RelatedToBargainInfoTemplate, bargain.Number, _shortDateFormatter.Format(bargain.CreatedOn));
        }
    }
}
