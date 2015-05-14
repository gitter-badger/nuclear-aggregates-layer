using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static UIElementMetadataBuilder CategoryFirmAddressesGrid()
        {
            return EntityGrid(EntityType.Instance.CategoryFirmAddress().Description.Pluralize(),
                              EntityType.Instance.CategoryFirmAddress(),
                              () => ErmConfigLocalization.CrdRelCategories)
                    .Icon.Path(Icons.Icons.Entity.Small(EntityType.Instance.Category()));
        }
    }
}