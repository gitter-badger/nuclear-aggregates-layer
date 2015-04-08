using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Deal =
            CardMetadata.For<Deal>()
                        .WithEntityIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<Deal>(),
                                ToolbarElements.Update<Deal>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Deal>(),
                                ToolbarElements.UpdateAndClose<Deal>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Deal>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.Deals.CloseDeal(),
                                                           ToolbarElements.Deals.Reopen()
                                                                          .DisableOn<IDeactivatableAspect>(x => x.IsActive),
                                                           ToolbarElements.Deals.ChangeClient(),
                                                           ToolbarElements.ChangeOwner<Deal>()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityName.Deal)),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, Icons.Icons.Entity.Small(EntityName.Order), () => ErmConfigLocalization.CrdRelOrders),
                                          RelatedItems.RelatedItem.ActivitiesGrid(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.FirmDeal, () => ErmConfigLocalization.CrdRelFirms)
                                                      .AppendapleEntity<Firm>());
    }
}