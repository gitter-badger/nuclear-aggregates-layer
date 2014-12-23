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
        public static readonly CardMetadata Deal =
            CardMetadata.For<Deal>()
                        .MainAttribute<Deal, IDealViewModel>(x => x.Name)
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
                                                           ToolbarElements.Deals.Reopen(),
                                                           ToolbarElements.Deals.ChangeClient(),
                                                           ToolbarElements.ChangeOwner<Deal>()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_Deal.gif"),
                                          UIElementMetadata.Config
                                                           .Name.Static("Orders")
                                                           .Icon.Path("en_ico_16_Order.gif")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Order)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Actions")
                                                           .Icon.Path("en_ico_16_Action.gif")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                           .Handler.ShowGridByConvention(EntityName.Activity)
                                                           .FilterToParents()
                                                           .LockOnNew(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Firms")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.FirmDeal)
                                                           .FilterToParent()
                                                           .AppendapleEntity<Firm>());
    }
}