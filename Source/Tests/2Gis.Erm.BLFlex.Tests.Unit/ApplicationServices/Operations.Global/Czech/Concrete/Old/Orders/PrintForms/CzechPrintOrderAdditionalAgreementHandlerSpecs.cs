﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public static class CzechPrintOrderAdditionalAgreementHandlerSpecs
    {
        class TestableCzechPrintOrderAdditionalAgreementHandler : CzechPrintOrderAdditionalAgreementHandler
        {
            public TestableCzechPrintOrderAdditionalAgreementHandler()
                : base(null, Create.FinderForPrintForms())
            {
            }

            public PrintData GetPrintData()
            {
                return GetPrintData(0, 0);
            }
        }

        [Tags("BL", "Print", "Czech")]
        [Subject(typeof(CzechPrintOrderAdditionalAgreementHandler))]
        public sealed class WhenGeneratingDataForAdditionalAgreement
        {
            #region PrintFormFields
            static readonly IDictionary<string, string[]> PrintFormFields = new Dictionary<string, string[]>
                {
                    {
                        "Dod. dohoda pro pr. osobu.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "OperatesOnTheBasis",
                                "Order.Number",
                                "Order.SignupDate",
                                "Profile.ChiefNameInGenitive",
                                "Profile.ChiefNameInNominative",
                                "Profile.Registered",
                            }
                    },
                    {
                        "Dod. dohoda pro SP.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "Order.Number",
                                "Order.SignupDate",
                                "Profile.ChiefNameInNominative",
                            }
                    },
                    {
                        "Dohoda o zruseni objednavky pro OSVC.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "Profile.ChiefNameInNominative",
                            }
                    },
                    {
                        "Dohoda o zruseni objednavky pro pr. osobu.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.ChiefNameInNominative",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "OperatesOnTheBasis",
                                "Profile.ChiefNameInGenitive",
                                "Profile.ChiefNameInNominative",
                                "Profile.Registered",
                            }
                    },
                }; 
            #endregion

            static PrintData Data;
            static TestableCzechPrintOrderAdditionalAgreementHandler Handler;
            static readonly string[] Expected = PrintFormFields.SelectMany(pair => pair.Value).Distinct().ToArray();

            Establish context = () => Handler = new TestableCzechPrintOrderAdditionalAgreementHandler();
            Because of = () => Data = Handler.GetPrintData();
            It should_contain_certain_fields = () => Data.Select(x => x.Key).Should().Contain(Expected);
            It should_not_contain_excessive_fields = () => Data.Select(x => x.Key).Should().BeSubsetOf(Expected);
        }
    }
}
