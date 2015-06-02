using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata FirmAddress =
            CardMetadata.For<FirmAddress>()
                        .WithDefaultIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.FirmContact(), () => ErmConfigLocalization.CrdRelFirmContacts),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.CategoryFirmAddress(), () => ErmConfigLocalization.CrdRelFirmAddressCategories));
    }
}