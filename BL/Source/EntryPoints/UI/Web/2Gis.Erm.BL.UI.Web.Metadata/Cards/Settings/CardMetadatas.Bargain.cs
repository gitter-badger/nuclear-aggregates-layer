using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Bargain =
            CardMetadata.For<Bargain>()
                        .WithEntityIcon()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.BargainFile, () => ErmConfigLocalization.CrdRelBargainFiles),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, () => ErmConfigLocalization.CrdRelOrders));
    }
}