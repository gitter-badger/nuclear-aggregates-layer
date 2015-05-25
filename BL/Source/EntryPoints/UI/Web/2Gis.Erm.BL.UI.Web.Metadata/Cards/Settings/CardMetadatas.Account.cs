using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityType.Instance.Account()))
                        .WarningOn<IDeactivatableAspect>(x => !x.IsActive, StringResourceDescriptor.Create(() => BLResources.AccountIsInactiveAlertText))
                        .ErrorOn<IDeletableAspect>(x => x.IsDeleted, StringResourceDescriptor.Create(() => BLResources.AccountIsDeletedAlertText))
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.AccountDetail(), () => ErmConfigLocalization.CrdRelAccountDetails),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Lock(), () => ErmConfigLocalization.CrdRelLocks),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Limit(), () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Order(), Icons.Icons.Entity.Small(EntityType.Instance.Order()), () => ErmConfigLocalization.CrdRelOrders));
    }
}