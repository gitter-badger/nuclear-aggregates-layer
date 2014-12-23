using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class PricePositions
        {
            public static UIElementMetadataBuilder Copy()
            {
                return 
                    
                    // COMMENT {all, 01.12.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("CopyPricePosition")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCopyPricePosition)
                                     .ControlType(ControlType.TextButton)
                                     .Handler.Name("scope.CopyPricePosition")
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Operation.NonCoupled<CopyPricePositionIdentity>();
            }
        }
    }
}