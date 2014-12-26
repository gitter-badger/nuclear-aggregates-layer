using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityName.Account))
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.AccountDetail, () => ErmConfigLocalization.CrdRelAccountDetails),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Lock, () => ErmConfigLocalization.CrdRelLocks),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Limit, () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, Icons.Icons.Entity.Small(EntityName.Order), () => ErmConfigLocalization.CrdRelOrders));
    }
}