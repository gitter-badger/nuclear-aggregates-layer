using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public static class CzechPrintHelper
    {
        public static PrintData BranchOfficeFields(BranchOffice branchOffice)
        {
            return new PrintData
                {
                    { "Ic", branchOffice.Ic },
                    { "Inn", branchOffice.Inn },
                    { "LegalAddress", branchOffice.LegalAddress },
                    { "Name", branchOffice.Name },
                };
        }

        public static PrintData BranchOfficeOrganizationUnitFields(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                {
                    { "Registered", branchOfficeOrganizationUnit.Registered },
                };
        }

        public static PrintData BranchOfficeOrganizationUnitFieldsForTerminationNotice(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                {
                    { "ChiefNameInGenitive", branchOfficeOrganizationUnit.ChiefNameInGenitive },
                    { "PositionInGenitive", branchOfficeOrganizationUnit.PositionInGenitive },
                    { "Registered", branchOfficeOrganizationUnit.Registered },
                };
        }

        public static PrintData BranchOfficeOrganizationUnitFieldsForAdditionalAgreement(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit)
        {
            return new PrintData
                {
                    { "ChiefNameInGenitive", branchOfficeOrganizationUnit.ChiefNameInGenitive },
                    { "ChiefNameInNominative", branchOfficeOrganizationUnit.ChiefNameInNominative },
                    { "PositionInGenitive", branchOfficeOrganizationUnit.PositionInGenitive },
                    { "Registered", branchOfficeOrganizationUnit.Registered },
                };
        }

        public static PrintData LegalPersonFields(LegalPerson legalPerson)
        {
            return new PrintData
                {
                    { "Ic", legalPerson.Ic },
                    { "Inn", legalPerson.Inn },
                    { "LegalAddress", legalPerson.LegalAddress },
                    { "LegalName", legalPerson.LegalName },
                    { "UseInn", !string.IsNullOrWhiteSpace(legalPerson.Inn) },
                };
        }

        public static PrintData OrderFieldsForLetterOfGuarantee(Order order)
        {
            return new PrintData
                {
                    { "Number", order.Number },
                };
        }

        public static PrintData OrderFields(Order order)
        {
            return new PrintData
                {
                    { "Number", order.Number },
                    { "SignupDate", order.SignupDate },
                };
        }

        public static PrintData LegalPersonProfileFields(LegalPersonProfile profile)
        {
            return new PrintData
                {
                    { "ChiefNameInGenitive", profile.ChiefNameInGenitive },
                    { "ChiefNameInNominative", profile.ChiefNameInNominative },
                };
        }

        public static PrintData LegalPersonProfileFieldsForAdditionalAgreement(LegalPersonProfile profile)
        {
            return new PrintData
                {
                    { "ChiefNameInGenitive", profile.ChiefNameInGenitive },
                    { "ChiefNameInNominative", profile.ChiefNameInNominative },
                    { "Registered", profile.Registered },
                };
        }
    }
}
