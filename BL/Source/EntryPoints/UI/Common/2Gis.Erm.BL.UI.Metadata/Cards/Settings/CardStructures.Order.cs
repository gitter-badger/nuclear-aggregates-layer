using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Order =
            CardMetadata.For<Order>()
                        .Title.Resource(() => ErmConfigLocalization.EnOrders)
                //.Parts.Use(
                //           () => BLResources.TitlePlacement,
                //           () => BLResources.TitleDiscount,
                //           () => BLResources.TitleFinances,
                //           () => BLResources.TitleCancellation,
                //           () => BLResources.TitleControl,
                //           () => BLResources.AdministrationTabTitle)
                        .Actions.Attach(UiElementMetadata.Config
                                                         .Title.Resource(() => ErmConfigLocalization.ControlSave)
                                                         .Operation.SpecificFor<ModifyBusinessModelEntityIdentity, Order>(),
                                        UiElementMetadata.Config
                                                         .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                                                         .Operation.SpecificFor<ModifyBusinessModelEntityIdentity, Order>()
                                                         .Operation.NonCoupled<CloseIdentity>(),
                                        UiElementMetadata.Config
                                                         .Title.Resource(() => ErmConfigLocalization.ControlRefresh)
                                                         .Operation.SpecificFor<GetDomainEntityDtoIdentity, Order>(),
                                        UiElementMetadata.Config
                                                         .Title.Resource(() => ErmConfigLocalization.ControlAssign)
                                                         .Operation.SpecificFor<AssignIdentity, Order>(),
                                        UiElementMetadata.Config
                                                         .Title.Resource(() => ErmConfigLocalization.ControlClose)
                                                         .Operation.NonCoupled<CloseIdentity>(),
                                        UiElementMetadata.Config
                                                         .Title.Resource(() => ErmConfigLocalization.ControlActions)
                                                         .Childs(
                                                                 UiElementMetadata.Config
                                                                                  .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                                                                  .Operation.SpecificFor<PrintIdentity, Order>()));

        //.RelatedItems.Attach(
        //                     HierarchyMetadata.Config
        //                                      .Title.Resource(() => ErmConfigLocalization.CrdRelBills)
        //                                      .Handler.ShowGrid(OrderAggregate.Bill.AsEntityName(), "OrderId={Id}", "Id == 0"),
        //                     HierarchyMetadata.Config
        //                                      .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
        //                                      .Handler.ShowGrid(AccountAggregate.Lock.AsEntityName(), "OrderId={Id}", "Id == 0"),
        //                     HierarchyMetadata.Config
        //                                      .Title.Resource(() => ErmConfigLocalization.CrdRelOrderFiles)
        //                                      .Handler.ShowGrid(OrderAggregate.OrderFile.AsEntityName(), "OrderId={Id}", "Id == 0"));

    }
}
