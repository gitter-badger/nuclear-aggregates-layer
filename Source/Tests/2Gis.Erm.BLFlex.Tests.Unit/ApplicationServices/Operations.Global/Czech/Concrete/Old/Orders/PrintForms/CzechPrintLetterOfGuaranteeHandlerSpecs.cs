using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public static class CzechPrintLetterOfGuaranteeHandlerSpecs
    {
        class TestableCzechPrintLetterOfGuaranteeHandler : CzechPrintLetterOfGuaranteeHandler
        {
            public TestableCzechPrintLetterOfGuaranteeHandler() 
                : base(null, Create.FinderForPrintForms())
            {
            }

            public PrintData GetPrintData()
            {
                return GetPrintData(0, 0);
            }
        }

        [Tags("BL", "Print", "Czech")]
        [Subject(typeof(CzechPrintLetterOfGuaranteeHandler))]
        public sealed class WhenGeneratingDataForLetterOfGuarantee
        {
            #region PrintFormFields
            static readonly IDictionary<string, string[]> PrintFormFields = new Dictionary<string, string[]>
                {
                    {
                        "Cestne prohlaseni Objednavatele.docx", new[]
                            {
                                "LegalPerson.UseInn",
                                "BranchOffice.Ic",
                                "BranchOffice.Inn",
                                "BranchOffice.LegalAddress",
                                "BranchOffice.Name",
                                "BranchOfficeOrganizationUnit.Registered",
                                "LegalAddressPrefix",
                                "LegalPerson.Ic",
                                "LegalPerson.Inn",
                                "LegalPerson.LegalAddress",
                                "LegalPerson.LegalName",
                                "OperatesOnTheBasis",
                                "Order.Number",
                                "PersonPrefix",
                                "Profile.ChiefNameInGenitive",
                                "Profile.ChiefNameInNominative",
                            }
                    }
                }; 
            #endregion

            static PrintData Data;
            static TestableCzechPrintLetterOfGuaranteeHandler Handler;
            static readonly string[] Expected = PrintFormFields.SelectMany(pair => pair.Value).Distinct().ToArray();

            Establish context = () => Handler = new TestableCzechPrintLetterOfGuaranteeHandler();
            Because of = () => Data = Handler.GetPrintData();
            It should_contain_certain_fields = () => Data.Select(x => x.Key).Should().Contain(Expected);
            It should_not_contain_excessive_fields = () => Data.Select(x => x.Key).Should().BeSubsetOf(Expected);
        }
    }
}
