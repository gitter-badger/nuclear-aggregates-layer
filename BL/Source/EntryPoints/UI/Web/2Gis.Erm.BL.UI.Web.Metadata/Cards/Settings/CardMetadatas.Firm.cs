using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Firm =
            CardMetadata.For<Firm>()
                        .MainAttribute<Firm, IFirmViewModel>(x => x.Name)
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
                                                           ToolbarElements.ChangeTerritory<Firm>(),
                                                           ToolbarElements.Firms.AssignWhiteListedAd(),
                                                           ToolbarElements.Qualify<Firm>()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config
                                                           .Name.Static("FirmAddresses")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAddresses)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.FirmAddress)
                                                           .FilterToParent(),

                                          UIElementMetadata.Config
                                                           .Name.Static("FirmCategories")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelCategories)
                                                           .Icon.Path("en_ico_16_Category.gif")
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.CategoryFirmAddress)
                                                           .FilterToParent(),

                                          UIElementMetadata.Config
                                                           .Name.Static("FirmAdvertisements")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAdvertisements)
                                                           .Icon.Path("en_ico_16_Advertisement.gif")
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Advertisement)
                                                           .FilterToParent(),

                                          UIElementMetadata.Config
                                                           .Name.Static("Orders")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                           .Icon.Path("en_ico_16_Order.gif")
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Order)
                                                           .FilterToParent(),

                                          UIElementMetadata.Config
                                                           .Name.Static("Actions")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                           .Icon.Path("en_ico_16_Action.gif")
                                                           .Handler.ShowGridByConvention(EntityName.Activity)
                                                           .FilterToParents()
                                                           .LockOnNew());
    }
}