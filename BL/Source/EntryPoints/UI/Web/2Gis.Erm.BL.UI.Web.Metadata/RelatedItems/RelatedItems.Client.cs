using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static class Client
        {
            public static UIElementMetadataBuilder ClientLinksGrid()
            {
                return UIElementMetadata.Config
                                        .Name.Static(EntityName.ClientLink.ToString().Pluralize())
                                        .Title.Resource(() => ErmConfigLocalization.CrdRelClientLinks)
                                        .Icon.Path(Icons.Icons.Entity.Small(EntityName.ClientLink))
                                        .LockOnNew()
                                        .Handler.ShowGridByConvention(EntityName.ClientLink);
            }
        }
    }
}
