using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Themes
        {
            public static UIElementMetadataBuilder SetAsDefault()
            {
                return UIElementMetadata.Config
                                        .Name.Static("SetDefaultTheme")
                                        .Title.Resource(() => ErmConfigLocalization.ControlSetDefaultTheme)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnInactive()
                                        .LockOnNew()
                                        .JSHandler("SetDefaultTheme")

                                        // COMMENT {all, 01.12.2014}: А зачем права на создание? 
                                        .AccessWithPrivelege<Theme>(EntityAccessTypes.Create)
                                        .AccessWithPrivelege<Theme>(EntityAccessTypes.Update)
                                        .Operation.NonCoupled<SetAsDefaultThemeIdentity>();
            }

            public static UIElementMetadataBuilder UnsetAsDefault()
            {
                return

                    UIElementMetadata.Config
                                     .Name.Static("UnSetDefaultTheme")
                                     .Title.Resource(() => ErmConfigLocalization.ControlUnSetDefaultTheme)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .JSHandler("UnSetDefaultTheme")

                                     // COMMENT {all, 01.12.2014}: А зачем права на создание? 
                                     .AccessWithPrivelege<Theme>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<Theme>(EntityAccessTypes.Update)
                                     .Operation.NonCoupled<SetAsDefaultThemeIdentity>();
            }
        }
    }
}
