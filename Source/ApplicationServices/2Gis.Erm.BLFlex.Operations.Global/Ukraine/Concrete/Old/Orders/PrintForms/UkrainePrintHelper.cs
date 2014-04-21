using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
            var branchOfficePart = branchOffice.UkrainePart();
            return 
                new PrintData
                    {
                        { "Ipn", branchOfficePart.Ipn },
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
            var part = legalPerson.UkrainePart();

            return new PrintData
                {
                    { "Egrpou", part.Egrpou },
                    { "Ipn", legalPerson.Inn },
                    { "LegalAddress", legalPerson.LegalAddress },
                    { "LegalName", legalPerson.LegalName },
                };
        }

        public static PrintData LegalPersonProfileFields(LegalPersonProfile profile)
        {
            var part = profile.UkrainePart();

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
                        ((PaymentMethod)profile.PaymentMethod).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                    },
                    { "AdditionalPaymentElements", profile.AdditionalPaymentElements },
                    { "Mfo", part.Mfo },
                };
        }

        public string GetOperatesOnTheBasisInGenitive(LegalPersonProfile profile)
        {
            if (profile == null || profile.OperatesOnTheBasisInGenitive == null)
            {
                return string.Empty;
            }

            switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
            {
                case OperatesOnTheBasisType.Charter:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCharterTemplate,
                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                    break;
                case OperatesOnTheBasisType.Certificate:
                    return string.Format(
                        BLResources.OperatesOnBasisOfCertificateTemplate,
                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.CertificateNumber,
                        _shortDateFormatter.Format(profile.CertificateDate.Value));
                    break;
                case OperatesOnTheBasisType.Warranty:
                    return string.Format(
                        BLResources.OperatesOnBasisOfWarantyTemplate,
                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                        profile.WarrantyNumber,
                        _shortDateFormatter.Format(profile.WarrantyBeginDate.Value));
                    break;
                case OperatesOnTheBasisType.FoundingBargain:
                    return string.Format(
                        BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                    break;
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
