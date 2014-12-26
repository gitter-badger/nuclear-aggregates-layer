using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class AdvertisementTemplates
        {
            public static UIElementMetadataBuilder Publish()
            {
                return UIElementMetadata.Config
                                        .Name.Static("PublishAdvertisementTemplate")
                                        .Title.Resource(() => ErmConfigLocalization.ControlPublishAdvertisementTemplate)
                                        .ControlType(ControlType.TextImageButton)
                                        .LockOnInactive()
                                        .LockOnNew()
                                        .AccessWithPrivelege(FunctionalPrivilegeName.PublishAdvertisementTemplate)
                                        .JSHandler("Publish")
                                        .Icon.Path(Icons.Icons.Toolbar.Refresh);
            }

            public static UIElementMetadataBuilder Unpublish()
            {
                return UIElementMetadata.Config
                                        .Name.Static("UnpublishAdvertisementTemplate")
                                        .Title.Resource(() => ErmConfigLocalization.ControlUnpublishAdvertisementTemplate)
                                        .ControlType(ControlType.TextImageButton)
                                        .LockOnNew()
                                        .AccessWithPrivelege(FunctionalPrivilegeName.UnpublishAdvertisementTemplate)
                                        .JSHandler("Unpublish")
                                        .Icon.Path(Icons.Icons.Toolbar.Refresh);
            }
        }
    }
}
