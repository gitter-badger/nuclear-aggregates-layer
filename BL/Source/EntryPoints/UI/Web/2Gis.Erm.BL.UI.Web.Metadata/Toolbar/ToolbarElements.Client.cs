using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Clients
        {
            public static UIElementMetadataBuilder Merge()
            {
                return UIElementMetadata.Config
                                        .Name.Static("Merge")
                                        .Title.Resource(() => ErmConfigLocalization.ControlMerge)
                                        .Icon.Path(Icons.Icons.Toolbar.Merge)
                                        .ControlType(ControlType.ImageButton)
                                        .LockOnNew()
                                        .LockOnInactive()
                                        .JSHandler("Merge")
                                        .AccessWithPrivelege(FunctionalPrivilegeName.MergeClients)
                                        .Operation.SpecificFor<MergeIdentity, Client>();
            }
        }
    }
}
