using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Firm =
            CardMetadata.For<Firm>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityName.Firm))
                        .Actions
                        .Attach(ToolbarElements.Create<Firm>(),
                                ToolbarElements.Update<Firm>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Firm>(),
                                ToolbarElements.UpdateAndClose<Firm>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Firm>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.ChangeOwner<Firm>(),
                                                           ToolbarElements.Firms.ChangeClient(),
                                                           ToolbarElements.ChangeTerritory<Firm>()
                                                                          .AccessWithPrivelege(FunctionalPrivilegeName.ChangeFirmTerritory),
                                                           ToolbarElements.Firms.AssignWhiteListedAd(),
                                                           ToolbarElements.Qualify<Firm>()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.FirmAddress, () => ErmConfigLocalization.CrdRelFirmAddresses),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.CategoryFirmAddress,
                                                                               Icons.Icons.Entity.Small(EntityName.Category),
                                                                               () => ErmConfigLocalization.CrdRelCategories),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Advertisement,
                                                                               Icons.Icons.Entity.Small(EntityName.Advertisement),
                                                                               () => ErmConfigLocalization.CrdRelFirmAdvertisements),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.Order, Icons.Icons.Entity.Small(EntityName.Order), () => ErmConfigLocalization.CrdRelOrders),
                                          RelatedItems.RelatedItem.ActivitiesGrid());
    }
}