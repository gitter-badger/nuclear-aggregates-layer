using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar
{
    public sealed partial class ToolbarElementsFlex
    {
        public static class Bargains
        {
            public static UIElementMetadataBuilder PrintBargain()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("PrintBargainAction")
                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnNew()
                                     .Handler.Name("scope.PrintBargain")
                                     .Operation.SpecificFor<PrintIdentity, Bargain>();
            }

            public static UIElementMetadataBuilder PrintBargainProlongation()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("PrintBargainProlongationAgreementAction")
                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainProlongationAgreementAction)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnNew()
                                     .Handler.Name("scope.PrintBargainProlongationAgreement");
            }

            public static class Russia
            {
                public static UIElementMetadataBuilder PrintNewSalesModelBargainAction()
                {
                    return
                        UIElementMetadata.Config
                                    .Name.Static("PrintNewSalesModelBargainAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintNewSalesModelBargain")
                                    .Operation.SpecificFor<PrintIdentity, Bargain>();
                }
            }
        }
    }
}
