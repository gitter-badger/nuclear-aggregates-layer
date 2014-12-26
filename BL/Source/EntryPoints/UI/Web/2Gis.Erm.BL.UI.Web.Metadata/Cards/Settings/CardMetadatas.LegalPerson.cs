using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata LegalPerson =
            CardMetadata.For<LegalPerson>()
                        .WithDefaultIcon()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Account, Icons.Icons.Entity.Account, () => ErmConfigLocalization.CrdRelAccounts),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Limit, () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Bargain, Icons.Icons.Entity.BargainSmall, () => ErmConfigLocalization.CrdRelBargains),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, Icons.Icons.Entity.Order, () => ErmConfigLocalization.CrdRelOrders));
    }
}