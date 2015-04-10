using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class OrganizationUnits
        {
            public static UIElementMetadataBuilder ManageCategories()
            {
                return

                    // COMMENT {all, 01.12.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("ManageCategories")
                                     .Title.Resource(() => ErmConfigLocalization.ControlManageCategories)
                                     .ControlType(ControlType.TextImageButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .JSHandler("ManageCategories")
                                     .Icon.Path(Icons.Icons.Entity.Small(EntityName.Category));
            }
        }
    }
}
