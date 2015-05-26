using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Firm =
            CardMetadata.For<Firm>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityType.Instance.Firm()))
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
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.FirmAddress(), () => ErmConfigLocalization.CrdRelFirmAddresses),
                                          RelatedItems.RelatedItem.CategoryFirmAddressesGrid(),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Advertisement(),
                                                                               Icons.Icons.Entity.Small(EntityType.Instance.Advertisement()),
                                                                               () => ErmConfigLocalization.CrdRelFirmAdvertisements),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Order(), Icons.Icons.Entity.Small(EntityType.Instance.Order()), () => ErmConfigLocalization.CrdRelOrders),
                                          RelatedItems.RelatedItem.ActivitiesGrid());
    }
}