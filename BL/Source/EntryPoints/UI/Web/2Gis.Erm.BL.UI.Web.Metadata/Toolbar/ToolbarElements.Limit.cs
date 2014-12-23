using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Limits
        {
            public static UIElementMetadataBuilder Approve()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("ApproveLimit")
                                     .Title.Resource(() => ErmConfigLocalization.ControlApproveLimit)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Handler.Name("scope.ApproveLimit");
            }

            public static UIElementMetadataBuilder Reject()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("RejectLimit")
                                     .Title.Resource(() => ErmConfigLocalization.ControlRejectLimit)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Handler.Name("scope.RejectLimit");
            }

            public static UIElementMetadataBuilder Open()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("OpenLimit")
                                     .Title.Resource(() => ErmConfigLocalization.ControlOpenLimit)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Handler.Name("scope.OpenLimit");
            }

            public static UIElementMetadataBuilder Recalculate()
            {
                return UIElementMetadata.Config
                                        .Name.Static("RecalculateLimit")
                                        .Title.Resource(() => ErmConfigLocalization.ControlRecalculateLimit)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnInactive()
                                        .Handler.Name("scope.RecalculateLimit")
                                        .AccessWithPrivelege(FunctionalPrivilegeName.LimitRecalculation);
            }
        }
    }
}
