using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static class Client
        {
            public static UIElementMetadataBuilder ClientLinksGrid()
            {
                return UIElementMetadata.Config
                                        .Name.Static(EntityType.Instance.ClientLink().ToString().Pluralize())
                                        .Title.Resource(() => ErmConfigLocalization.CrdRelClientLinks)
                                        .Icon.Path(Icons.Icons.Entity.Small(EntityType.Instance.ClientLink()))
                                        .LockOnNew()
                                        .Handler.ShowGridByConvention(EntityType.Instance.ClientLink());
            }
        }
    }
}
