using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static UIElementMetadataBuilder CategoryFirmAddressesGrid()
        {
            return EntityGrid(EntityName.CategoryFirmAddress.ToString().Pluralize(),
                              EntityName.CategoryFirmAddress,
                              () => ErmConfigLocalization.CrdRelCategories)
                    .Icon.Path(Icons.Icons.Entity.Small(EntityName.Category));
        }
    }
}