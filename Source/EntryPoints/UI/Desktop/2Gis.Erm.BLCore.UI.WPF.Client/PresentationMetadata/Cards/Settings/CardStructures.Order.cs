using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards.Generated;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public readonly static CardStructure Order =
            CardStructure.Config
                .For(OrderAggregate.Order.AsEntityName())
                .Title.Resource(() => ErmConfigLocalization.EnOrders)
                .Parts.Use(
                    () => BLResources.TitlePlacement,
                    () => BLResources.TitleDiscount,
                    () => BLResources.TitleFinances,
                    () => BLResources.TitleCancellation,
                    () => BLResources.TitleControl,
                    () => BLResources.AdministrationTabTitle)
                .Actions.Attach(
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlSave)
                        .Operation.EntitySpecific<ModifyBusinessModelEntityIdentity>(OrderAggregate.Order.AsEntityName()),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                        .Operation.EntitySpecific<ModifyBusinessModelEntityIdentity>(OrderAggregate.Order.AsEntityName())
                        .Operation.NonCoupled<CloseIdentity>(),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlRefresh)
                        .Operation.EntitySpecific<GetDomainEntityDtoIdentity>(OrderAggregate.Order.AsEntityName()),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlAssign)
                        .Operation.EntitySpecific<AssignIdentity>(OrderAggregate.Order.AsEntityName()),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlClose)
                        .Operation.EntitySpecific<CloseIdentity>(OrderAggregate.Order.AsEntityName()),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlActions)
                        .Childs(
                            HierarchyElement.Config
                                .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                .Operation.EntitySpecific<PrintIdentity>(OrderAggregate.Order.AsEntityName())))
                .RelatedItems.Attach(
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.CrdRelBills)
                        .Handler.ShowGrid(OrderAggregate.Bill.AsEntityName(), "OrderId={Id}", "Id == 0"),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
                        .Handler.ShowGrid(AccountAggregate.Lock.AsEntityName(), "OrderId={Id}", "Id == 0"),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.CrdRelOrderFiles)
                        .Handler.ShowGrid(OrderAggregate.OrderFile.AsEntityName(), "OrderId={Id}", "Id == 0"))
                .MVVM.Bind<DynamicCardViewModel, OrderView>()
                .WithDynamicProperties()
                .Validator.Dynamic<DynamicViewModelValidator<DynamicViewModel>, DynamicViewModel>()
                //.Validator.Static<ValidationTestValidator, ValidationTestViewModel>()
                //.Validator.Static<ValidationTestValidator, ValidationTestViewModel>()
                .Localizator(typeof(MetadataResources), typeof(BLResources), typeof(EnumResources));

    }
}
