using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Views.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;

using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public readonly static CardMetadata Order =
            CardMetadata.Config
                .For<Order>()
                .Title.Resource(() => ErmConfigLocalization.EnOrders)
                .Parts.Use(
                    () => BLResources.TitlePlacement,
                    () => BLResources.TitleDiscount,
                    () => BLResources.TitleFinances,
                    () => BLResources.TitleCancellation,
                    () => BLResources.TitleControl,
                    () => BLResources.AdministrationTabTitle)
                .Actions.Attach(
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlSave)
                        .Operation.SpecificFor<ModifyBusinessModelEntityIdentity, Order>(),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                        .Operation.SpecificFor<ModifyBusinessModelEntityIdentity, Order>()
                        .Operation.NonCoupled<CloseIdentity>(),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlRefresh)
                        .Operation.SpecificFor<GetDomainEntityDtoIdentity, Order>(),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlAssign)
                        .Operation.SpecificFor<AssignIdentity, Order>(),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlClose)
                        .Operation.NonCoupled<CloseIdentity>(),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.ControlActions)
                        .Childs(
                            OldUIElementMetadata.Config
                                .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                .Operation.SpecificFor<PrintIdentity, Order>()))
                .RelatedItems.Attach(
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.CrdRelBills)
                        .Handler.ShowGrid(EntityType.Instance.Bill(), "OrderId={Id}", "Id == 0"),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
                        .Handler.ShowGrid(EntityType.Instance.Lock(), "OrderId={Id}", "Id == 0"),
                    OldUIElementMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.CrdRelOrderFiles)
                        .Handler.ShowGrid(EntityType.Instance.OrderFile(), "OrderId={Id}", "Id == 0"))
                .MVVM.Bind<DynamicCardViewModel, OrderView>()
                .WithDynamicProperties()
                .Validator.Dynamic<DynamicViewModelValidator<DynamicViewModel>, DynamicViewModel>()
                //.Validator.Static<ValidationTestValidator, ValidationTestViewModel>()
                //.Validator.Static<ValidationTestValidator, ValidationTestViewModel>()
                .Localizator(typeof(MetadataResources), typeof(BLResources), typeof(EnumResources));

    }
}
