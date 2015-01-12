using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
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
                                     .JSHandler("ApproveLimit");
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
                                     .JSHandler("RejectLimit");
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
                                     .JSHandler("OpenLimit");
            }

            public static UIElementMetadataBuilder Recalculate()
            {
                return UIElementMetadata.Config
                                        .Name.Static("RecalculateLimit")
                                        .Title.Resource(() => ErmConfigLocalization.ControlRecalculateLimit)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnInactive()
                                        .JSHandler("RecalculateLimit")
                                        .AccessWithPrivelege(FunctionalPrivilegeName.LimitRecalculation);
            }

            public static UIElementMetadataBuilder Increase()
            {
                return UIElementMetadata.Config
                                        .Name.Static("IncreaseLimit")
                                        .Title.Resource(() => ErmConfigLocalization.ControlIncreaseLimit)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnNew()
                                        .JSHandler("IncreaseLimit")
                                        .AccessWithPrivelege(FunctionalPrivilegeName.LimitManagement);
            }
        }
    }
}
