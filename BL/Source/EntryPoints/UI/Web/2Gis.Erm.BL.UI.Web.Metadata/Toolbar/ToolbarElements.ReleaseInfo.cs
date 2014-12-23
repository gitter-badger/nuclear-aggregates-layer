using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class ReleaseInfos
        {
            // COMMENT {all, 23.12.2014}: Есть подозрение, что нужно объединить с SaveAs (LocalMessage)
            public static UIElementMetadataBuilder Download()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("DownloadResults")
                                     .Title.Resource(() => ErmConfigLocalization.ControlDownloadResults)
                                     .ControlType(ControlType.TextButton)
                                     .Handler.Name("scope.DownloadResults")
                                     .LockOnNew();
            }
        }
    }
}
