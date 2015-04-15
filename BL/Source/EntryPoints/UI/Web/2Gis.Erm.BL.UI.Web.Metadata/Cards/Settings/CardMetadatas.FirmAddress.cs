using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata FirmAddress =
            CardMetadata.For<FirmAddress>()
                        .WithDefaultIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.FirmContact, () => ErmConfigLocalization.CrdRelFirmContacts),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.CategoryFirmAddress, () => ErmConfigLocalization.CrdRelFirmAddressCategories));
    }
}