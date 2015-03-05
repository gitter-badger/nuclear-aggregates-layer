using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic
{
    public sealed class BargainPrintFormDataExtractor : IBargainPrintFormDataExtractor
    {
        public PrintData GetBranchOffice(IQueryable<BranchOffice> query)
        {
            var branchOffice = query
                .Select(office => new
                {
                    office.Inn,
                    office.LegalAddress,
                })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Inn", x.Inn },
                        { "LegalAddress", x.LegalAddress },
                    })
                .Single();

            return new PrintData
                {
                    { "BranchOffice", branchOffice },
                };
        }

        public PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou)
        {
            var branchOfficeOrganizationUnit = new PrintData
                {
                    { "ChiefNameInGenitive", boou.ChiefNameInGenitive },
                    { "ChiefNameInNominative", boou.ChiefNameInNominative },
                    { "OperatesOnTheBasisInGenitive", boou.OperatesOnTheBasisInGenitive },
                    { "Kpp", boou.Kpp },
                    { "PaymentEssentialElements", boou.PaymentEssentialElements },
                    { "PositionInGenitive", boou.PositionInGenitive },
                    { "PositionInNominative", boou.PositionInNominative },
                    { "ShortLegalName", boou.ShortLegalName },
                    { "ActualAddress", boou.ActualAddress },
                    { "Email", boou.Email },
                    { "PhoneNumber", boou.PhoneNumber },
                };

            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", branchOfficeOrganizationUnit },
                };
        }

        public PrintData GetLegalPerson(LegalPerson legalPerson)
        {
            var legalPersonData = new PrintData
                {
                    { "Inn", legalPerson.Inn },
                    { "Kpp", legalPerson.Kpp },
                    { "VAT", legalPerson.VAT },
                    { "LegalAddress", legalPerson.LegalAddress },
                    { "LegalName", legalPerson.LegalName },
                    { "PassportIssuedBy", legalPerson.PassportIssuedBy },
                    { "PassportNumber", legalPerson.PassportNumber },
                    { "PassportSeries", legalPerson.PassportSeries },
                    { "RegistrationAddress", legalPerson.RegistrationAddress },
                    { "ShortName", legalPerson.ShortName },
                };

            return new PrintData
                {
                    { "LegalPerson", legalPersonData },
                };
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            var operatesOnTheBasis = legalPersonProfile.OperatesOnTheBasisInGenitive.Value;

            var profileData = new PrintData
                {
                    { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                    { "PaymentEssentialElements", legalPersonProfile.PaymentEssentialElements },
                    { "PositionInNominative", legalPersonProfile.PositionInNominative },
                    { "ChiefNameInGenitive", legalPersonProfile.ChiefNameInGenitive },
                    { "PositionInGenitive", legalPersonProfile.PositionInGenitive },
                    { "DocumentName", operatesOnTheBasis.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture) },
                    { "CertificateNumber", legalPersonProfile.CertificateNumber },
                    { "CertificateDate", legalPersonProfile.CertificateDate },
                    { "WarrantyNumber", legalPersonProfile.WarrantyNumber },
                    { "WarrantyBeginDate", legalPersonProfile.WarrantyBeginDate },
                    { "BargainNumber", legalPersonProfile.BargainNumber },
                    { "BargainBeginDate", legalPersonProfile.BargainBeginDate },
                    { "IBAN", legalPersonProfile.IBAN },
                    { "SWIFT", legalPersonProfile.SWIFT },
                    { "AccountNumber", legalPersonProfile.AccountNumber },
                    { "BankName", legalPersonProfile.BankName },
                    { "Phone", legalPersonProfile.Phone },
                    { "Email", legalPersonProfile.Email },
                };

            return new PrintData
                {
                    { "UseBargain", operatesOnTheBasis == OperatesOnTheBasisType.Bargain },
                    { "UseCertificate", operatesOnTheBasis == OperatesOnTheBasisType.Certificate },
                    { "UseCharter", operatesOnTheBasis == OperatesOnTheBasisType.Charter },
                    { "UseFoundingBargain", operatesOnTheBasis == OperatesOnTheBasisType.FoundingBargain },
                    { "UseRegistrationCertificate", operatesOnTheBasis == OperatesOnTheBasisType.RegistrationCertificate },
                    { "UseWarranty", operatesOnTheBasis == OperatesOnTheBasisType.Warranty },
                    { "LegalPersonProfile", profileData },
                };
        }

        public PrintData GetUngroupedFields(IQueryable<Bargain> bargainQuery)
        {
            return bargainQuery
                .Select(x => new
                    {
                        LegalPersonType = x.LegalPerson.LegalPersonTypeEnum,
                        OrganizationUnitName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Name,
                        EndDate = x.BargainEndDate
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "UseBusinessman", x.LegalPersonType == LegalPersonType.Businessman },
                        { "UseLegalPerson", x.LegalPersonType == LegalPersonType.LegalPerson },
                        { "UseNaturalPerson", x.LegalPersonType == LegalPersonType.NaturalPerson },
                        { "UseEndlessBargain", x.EndDate == null },
                        { "UseLimitedBargain", x.EndDate != null },
                        { "OrganizationUnitName", x.OrganizationUnitName },
                    })
                .Single();
        }

        public PrintData GetBargain(IQueryable<Bargain> queryable)
        {
            var bargainData = queryable
                .Select(bargain => new { bargain.Number, bargain.SignedOn, bargain.BargainEndDate })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Number", x.Number },
                        { "SignedOn", x.SignedOn },
                        { "EndDate", x.BargainEndDate },
                    })
                .Single();

            return new PrintData
                {
                    { "Bargain", bargainData }
                };
        }
    }
}