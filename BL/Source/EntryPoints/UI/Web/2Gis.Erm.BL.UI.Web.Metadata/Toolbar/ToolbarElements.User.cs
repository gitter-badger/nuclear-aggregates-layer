using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Users
        {
            public static UIElementMetadataBuilder Profile()
            {
                return 
                    
                    // COMMENT {all, 01.12.2014}: а как же безопасность? 
                    UIElementMetadata.Config
                                     .Name.Static("ShowUserProfile")
                                     .Title.Resource(() => ErmConfigLocalization.EnUserProfile)
                                     .LockOnNew()
                                     .ControlType(ControlType.TextButton)
                                     .JSHandler("ProcessUserProfile");
            }
        }
    }
}
