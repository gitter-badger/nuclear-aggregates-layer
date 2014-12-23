using DoubleGis.Erm.BL.Resources.Server.Properties;
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
                                        .Handler.Name("scope.Publish")
                                        .Icon.Path("Refresh.gif");
            }

            public static UIElementMetadataBuilder Unpublish()
            {
                return UIElementMetadata.Config
                                        .Name.Static("UnpublishAdvertisementTemplate")
                                        .Title.Resource(() => ErmConfigLocalization.ControlUnpublishAdvertisementTemplate)
                                        .ControlType(ControlType.TextImageButton)
                                        .LockOnNew()
                                        .AccessWithPrivelege(FunctionalPrivilegeName.UnpublishAdvertisementTemplate)
                                        .Handler.Name("scope.Unpublish")
                                        .Icon.Path("Refresh.gif");
            }
        }
    }
}
