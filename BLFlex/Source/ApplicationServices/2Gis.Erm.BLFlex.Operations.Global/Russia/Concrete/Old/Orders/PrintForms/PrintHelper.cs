using System;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
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

                           { "BranchOffice.Name", branchOffice.Inn },
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

        public static PrintData AgreementBody(Order order, LegalPerson legalPerson, LegalPersonProfile legalPersonProfile, BranchOfficeOrganizationUnit branchOfficeOrganizationUnit, Firm firm)
        {
            return new PrintData
                       {
                           { "Now", DateTime.Today },
                           { "Order.Number", order.Number },
                           { "Order.CreatedOn", order.CreatedOn },
                           { "LegalPerson.LegalName", legalPerson.LegalName },
                           { "OrganizationUnitName", "ToDo" }, // FIXME {a.rechkalov, 19.03.2015}: закончить после ERM-6067

                           { "LegalPersonProfile.PositionInGenitive", legalPersonProfile.PositionInGenitive },
                           { "LegalPersonProfile.ChiefNameInGenitive", legalPersonProfile.ChiefNameInGenitive },
                           { "LegalPersonProfile.OperatesOnTheBasisInGenitive", legalPersonProfile.OperatesOnTheBasisInGenitive },

                           { "BranchOfficeOrganizationUnit.ShortLegalName", branchOfficeOrganizationUnit.ShortLegalName },
                           { "BranchOfficeOrganizationUnit.PositionInGenitive", branchOfficeOrganizationUnit.PositionInGenitive },
                           { "BranchOfficeOrganizationUnit.ChiefNameInGenitive", branchOfficeOrganizationUnit.ChiefNameInGenitive },
                           { "BranchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive", branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive },

                           { "Firm.Name", firm.Name },
                       };
        }
    }
}
