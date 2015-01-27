using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityName.Account))
                        .WarningOn<IDeactivatableAspect>(x => !x.IsActive, StringResourceDescriptor.Create(() => BLResources.AccountIsInactiveAlertText))
                        .ErrorOn<IDeletableAspect>(x => x.IsDeleted, StringResourceDescriptor.Create(() => BLResources.AccountIsDeletedAlertText))
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.AccountDetail, () => ErmConfigLocalization.CrdRelAccountDetails),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Lock, () => ErmConfigLocalization.CrdRelLocks),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Limit, () => ErmConfigLocalization.CrdRelLimits),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, Icons.Icons.Entity.Small(EntityName.Order), () => ErmConfigLocalization.CrdRelOrders));
    }
}