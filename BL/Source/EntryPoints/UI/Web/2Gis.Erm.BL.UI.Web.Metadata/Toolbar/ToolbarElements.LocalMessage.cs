using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class LocalMessages
        {
            // COMMENT {all, 23.12.2014}: Есть подозрение, что нужно объединить с Download (ReleasInfo), Download (WithdrawalInfo)
            public static UIElementMetadataBuilder SaveAs()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("SaveAs")
                                     .Title.Resource(() => ErmConfigLocalization.ControlSaveAs)
                                     .ControlType(ControlType.TextButton)
                                     .JSHandler("SaveAs");
            }
        }
    }
}
