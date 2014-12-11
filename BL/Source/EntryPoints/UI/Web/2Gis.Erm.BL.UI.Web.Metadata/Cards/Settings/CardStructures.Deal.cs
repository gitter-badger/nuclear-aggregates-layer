using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Deal =
            CardMetadata.For<Deal>()
                        .MainAttribute<Deal, IDealViewModel>(x => x.Name)
                        .Actions
                        .Attach(UiElementMetadata.Config.CreateAction<Deal>(),
                                UiElementMetadata.Config.UpdateAction<Deal>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CreateAndCloseAction<Deal>(),
                                UiElementMetadata.Config.UpdateAndCloseAction<Deal>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<Deal>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.AdditionalActions
                                    (

                                     // COMMENT {all, 28.11.2014}: а как же безопасность?
                                     UiElementMetadata.Config
                                                      .Name.Static("CloseDeal")
                                                      .Title.Resource(() => ErmConfigLocalization.ControlCloseDeal)
                                                      .ControlType(ControlType.TextButton)
                                                      .LockOnInactive()
                                                      .Handler.Name("scope.CloseDeal"),

                                     // COMMENT {all, 28.11.2014}: а как же безопасность?
                                     UiElementMetadata.Config
                                                      .Name.Static("ReopenDeal")
                                                      .Title.Resource(() => ErmConfigLocalization.ControlReopenDeal)
                                                      .ControlType(ControlType.TextButton)
                                                      .Handler.Name("scope.ReopenDeal"),

                                     // COMMENT {all, 28.11.2014}: а как же безопасность?
                                     UiElementMetadata.Config
                                                      .Name.Static("ChangeDealClient")
                                                      .Title.Resource(() => ErmConfigLocalization.ControlChangeDealClient)
                                                      .ControlType(ControlType.TextButton)
                                                      .LockOnInactive()
                                                      .Handler.Name("scope.ChangeDealClient")
                                                      .Operation.SpecificFor<ChangeClientIdentity, Deal>(),

                                     // COMMENT {all, 28.11.2014}: а почему не assign?
                                     UiElementMetadata.Config.ChangeOwnerAction<Deal>()),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab("en_ico_16_Deal.gif"),
                                            UiElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Actions")
                                                             .Icon.Path("en_ico_16_Action.gif")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                             .Handler.ShowGridByConvention(EntityName.Activity)
                                                             .FilterToParents()
                                                             .LockOnNew(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Firms")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.FirmDeal)
                                                             .FilterToParent()
                                                             .AppendapleEntity<Firm>());
    }
}