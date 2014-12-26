using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static UIElementMetadataBuilder SetAsPrimary()
            {
                return

                    // COMMENT {all, 27.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("SetAsPrimary")
                                     .Title.Resource(() => ErmConfigLocalization.ControlSetAsPrimary)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnNew()
                                     .LockOnInactive()
                                     .JSHandler("SetAsPrimary")
                                     .Operation.NonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryIdentity>();
            }

            public static UIElementMetadataBuilder SetAsPrimaryForRegSales()
            {
                return 
                    
                    // COMMENT {all, 27.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("SetAsPrimaryForRegSales")
                                     .Title.Resource(() => ErmConfigLocalization.ControlSetAsPrimaryForRegSales)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .JSHandler("SetAsPrimaryForRegSales")
                                     .Operation.NonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity>();
            }
        }
    }
}
