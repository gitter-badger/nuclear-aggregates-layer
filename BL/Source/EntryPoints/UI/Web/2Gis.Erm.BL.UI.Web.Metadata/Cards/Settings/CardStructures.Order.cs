using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Order =
            CardMetadata.For<Order>()
                        .MainAttribute<Order, IOrderViewModel>(x => x.OrderNumber)                
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Order>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Order>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Order>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.PrintActions(UiElementMetadata.Config
                                                                                           .Name.Static("PrintOrderAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintOrder")
                                                                                           .Operation.SpecificFor<PrintIdentity, Order>(),
                                                                          UiElementMetadata.Config
                                                                                           .Name.Static("PrintActionsAdditional")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                                                                           .ControlType(ControlType.Menu)
                                                                                           .Childs(UiElementMetadata.Config
                                                                                                                    .Name.Static("PrintBargainAction")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrintOrderBargain")
                                                                                                                    .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                                                                   UiElementMetadata.Config
                                                                                                                    .Name.Static("PrintNewSalesModelBargainAction")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrintNewSalesModelOrderBargain")
                                                                                                                    .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                                                                   UiElementMetadata.Config
                                                                                                                    .Name.Static("PrepareJointBillsAction")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrepareJointBillsAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrepareJointBill"),
                                                                                                   UiElementMetadata.Config
                                                                                                                    .Name.Static("PrintBillAction")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrintOrderBills")
                                                                                                                    .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                                                                   UiElementMetadata.Config
                                                                                                                    .Name.Static("PrintTerminationNoticeAction")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrintTerminationNotice"),
                                                                                                   UiElementMetadata.Config
                                                                                                                    .Name.Static("PrintAdditionalAgreementAction")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrintAdditionalAgreement"),
                                                                                                   UiElementMetadata.Config
                                                                                                                    .Name.Static("PrintLetterOfGuarantee")
                                                                                                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                                                                                                    .ControlType(ControlType.TextButton)
                                                                                                                    .Handler.Name("scope.PrintLetterOfGuarantee"))),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config
                                                                                                .Name.Static("ChangeDeal")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlChangeDeal)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.ChangeDeal")
                                                                                                 
                                                                                                // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                                                                                .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                                                                                .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                                                                                .Operation.NonCoupled<ChangeDealIdentity>(),
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("CheckOrder")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlCheckOrder)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.CheckOrder")
                                                                                                .Operation.NonCoupled<ValidateOrdersIdentity>(),
                                                                               UiElementMetadata.Config.ChangeOwnerAction<Order>(),
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("CloseWithDenial")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlCloseWithDenial)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.CloseWithDenial")

                                                                                                // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                                                                                .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                                                                                .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                                                                                .Operation.NonCoupled<CloseWithDenialIdentity>(),

                                                                               // COMMENT {all, 01.12.2014}: а как же безопасность?
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("SwitchToAccount")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlSwitchToAccount)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnInactive()
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.SwitchToAccount"),
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("CopyOrder")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlCopyOrder)
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .LockOnNew()
                                                                                                .Handler.Name("scope.CopyOrder")
                                                                                                .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                                                                                .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                                                                                .Operation.NonCoupled<CopyOrderIdentity>()),
                                    UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(
                                    UiElementMetadata.Config
                                                     .Name.Static("Bills")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelBills)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Bill)
                                                     .FilterToParent(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Locks")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Lock)
                                                     .FilterToParent(),
                                    UiElementMetadata.Config
                                                     .Name.Static("OrderFiles")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelOrderFiles)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.OrderFile)
                                                     .FilterToParent(),
                                    UiElementMetadata.Config
                                                     .Name.Static("OrderProcessingRequests")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelOrderProcessingRequests)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.OrderProcessingRequest)
                                                     .FilterToParent());
    }
}