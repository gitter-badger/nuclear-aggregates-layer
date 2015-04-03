using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Icons;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar
{
    public sealed partial class ToolbarElementsFlex
    {
        public static class Advertisements
        {
            public static UIElementMetadataBuilder Preview()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("Preview")
                                     .Title.Resource(() => ErmConfigLocalization.ControlPreviewAdvertisement)
                                     .ControlType(ControlType.TextImageButton)
                                     .LockOnNew()
                                     .JSHandler("Preview")
                                     .Icon.Path(Icons.Toolbar.PreviewAdvertisement);
            }
        }
    }
}