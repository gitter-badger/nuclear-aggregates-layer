using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar
{
    public sealed partial class ToolbarElementsFlex
    {
        public static class Bills
        {
            public static UIElementMetadataBuilder PrintBill()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("PrintBillAction")
                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillAction)
                                     .ControlType(ControlType.TextButton)
                                     .JSHandler("PrintBill")
                                     .LockOnNew()
                                     .Operation.SpecificFor<PrintIdentity, Bill>();
            }
        }
    }
}
