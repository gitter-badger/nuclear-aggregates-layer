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
        public static readonly CardMetadata LegalPerson =
            CardMetadata.For<LegalPerson>()
                        .WithDefaultIcon()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Account(), Icons.Icons.Entity.Small(EntityType.Instance.Account()), () => ErmConfigLocalization.CrdRelAccounts),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Limit(), () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Bargain(), Icons.Icons.Entity.Small(EntityType.Instance.Bargain()), () => ErmConfigLocalization.CrdRelBargains),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Order(), Icons.Icons.Entity.Small(EntityType.Instance.Order()), () => ErmConfigLocalization.CrdRelOrders));
    }
}