using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public static class CzechPrintOrderTerminationNoticeHandlerSpecs
    {
        class TestableCzechPrintOrderTerminationNoticeHandler : CzechPrintOrderTerminationNoticeHandler
        {
            public TestableCzechPrintOrderTerminationNoticeHandler()
                : base(null, Create.FinderForPrintForms())
            {
            }

            public PrintData GetPrintData()
            {
                return GetPrintData(0);
            }
        }

        [Tags("BL", "Print", "Czech")]
        [Subject(typeof(CzechPrintOrderTerminationNoticeHandler))]
        public sealed class WhenGeneratingDataForTerminationNotice
        {
            #region PrintFormFields
            static readonly IDictionary<string, string[]> PrintFormFields = new Dictionary<string, string[]>
                {
                    {
                        "Oznameni o zruseni objednavky bez uvedeni duvodu pro OSVC.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "Order.Number",
                                "Order.SignupDate",
                            }
                    },
                    {
                        "Oznameni o zruseni pro pr. osobu.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "Order.Number",
                                "Order.SignupDate",
                            }
                    },
                    {
                        "Oznameni o zruseni objednavky bez uvedeni duvodu pro pr. osobu.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "Order.Number",
                                "Order.SignupDate",
                            }
                    },
                    {
                        "Oznameni o zruseni pro SP.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "Order.Number",
                                "Order.SignupDate",
                            }
                    },
                    {
                        "Oznameni o zruseni smlouvy bez uvedeni duvodu pro OSVC.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                            }
                    },
                    {
                        "Oznameni o zruseni smlouvy bez uvedeni duvodu pro pr. osobu.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                            }
                    },
                    {
                        "Oznameni o zruseni smlouvy s uvedenim duvodu pro OSVC.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                            }
                    },
                    {
                        "Oznameni o zruseni smlouvy s uvedenim duvodu pro pr. osobu.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.ChiefNameInGenitive",
                                "BranchOfficeOrganizationUnit.PositionInGenitive",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                            }
                    },
                };
            
            #endregion

            static PrintData Data;
            static TestableCzechPrintOrderTerminationNoticeHandler Handler;
            static readonly string[] Expected = PrintFormFields.SelectMany(pair => pair.Value).Distinct().ToArray();

            Establish context = () => Handler = new TestableCzechPrintOrderTerminationNoticeHandler();
            Because of = () => Data = Handler.GetPrintData();
            It should_contain_certain_fields = () => Data.Select(x => x.Key).Should().Contain(Expected);
            It should_not_contain_excessive_fields = () => Data.Select(x => x.Key).Should().BeSubsetOf(Expected);
        }
    }
}
