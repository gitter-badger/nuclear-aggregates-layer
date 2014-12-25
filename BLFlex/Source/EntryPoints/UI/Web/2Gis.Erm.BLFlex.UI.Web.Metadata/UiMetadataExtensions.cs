using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc
{
    public static class UIMetadataExtensions
    {
        public static UIElementMetadata[] OrderAdditionalActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("ChangeDeal")
                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeDeal)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.ChangeDeal")
                                                                                             
                               // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                            .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                            .AccessWithPrivelege(FunctionalPrivilegeName.OrderChangeDealExtended)
                                            .Operation.NonCoupled<ChangeDealIdentity>(),
                           UIElementMetadata.Config
                                            .Name.Static("CheckOrder")
                                            .Title.Resource(() => ErmConfigLocalization.ControlCheckOrder)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.CheckOrder")
                                            .Operation.NonCoupled<ValidateOrdersIdentity>(),
                           ToolbarElements.ChangeOwner<Order>(),
                           UIElementMetadata.Config
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
                           UIElementMetadata.Config
                                            .Name.Static("SwitchToAccount")
                                            .Title.Resource(() => ErmConfigLocalization.ControlSwitchToAccount)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnInactive()
                                            .LockOnNew()
                                            .Handler.Name("scope.SwitchToAccount"),
                           UIElementMetadata.Config
                                            .Name.Static("CopyOrder")
                                            .Title.Resource(() => ErmConfigLocalization.ControlCopyOrder)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.CopyOrder")
                                            .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                            .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                            .Operation.NonCoupled<CopyOrderIdentity>()
                       };
        }

        public static UIElementMetadata[] CyprusOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"))
                       };
        }

        public static UIElementMetadata[] CzechOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeWithoutReasonAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeWithoutReasonAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNoticeWithoutReason"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationBargainNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermBargainNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationBargainNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationBargainNoticeWithoutReasonAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermBargainNoticeWithoutReasonAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationBargainNoticeWithoutReason"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargainAdditionalAgreement"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintSwornStatementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UIElementMetadata[] RussianOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintNewSalesModelBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintNewSalesModelOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrepareJointBillsAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrepareJointBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrepareJointBill"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UIElementMetadata[] ChileOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTerminationBargainNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UIElementMetadata[] UkraineOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UIElementMetadata[] EmiratesOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargainAdditionalAgreement"))
                       };
        }

        public static UIElementMetadata[] KazakhstanOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           UIElementMetadata.Config
                                            .Name.Static("PrintOrderAction")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnNew()
                                            .Handler.Name("scope.PrintOrder")
                                            .Operation.SpecificFor<PrintIdentity, Order>(),
                           UIElementMetadata.Config
                                            .Name.Static("PrintActionsAdditional")
                                            .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                            .ControlType(ControlType.Menu)
                                            .Childs(UIElementMetadata.Config
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UIElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UIElementMetadata[] CommonLegalPersonAdditionalActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           // COMMENT {all, 29.11.2014}: а как же безопасность?
                           UIElementMetadata.Config
                                            .Name.Static("ChangeLegalPersonClient")
                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeLegalPersonClient)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnInactive()
                                            .LockOnNew()
                                            .Handler.Name("scope.ChangeLegalPersonClient")
                                            .Operation.SpecificFor<ChangeClientIdentity, LegalPerson>(),

                           // COMMENT {all, 29.11.2014}: а как же безопасность?
                           UIElementMetadata.Config
                                            .Name.Static("ChangeLPRequisites")
                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeLPRequisites)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnInactive()
                                            .LockOnNew()
                                            .Handler.Name("scope.ChangeLegalPersonRequisites")
                                            .Operation.NonCoupled<ChangeRequisitesIdentity>()
                       };
        }

        public static UIElementMetadata[] CommonOrderRelatedActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                          UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Bills")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelBills)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Bill)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Locks")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Lock)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("OrderFiles")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrderFiles)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.OrderFile)
                                                             .FilterToParent()
                       };
        }

        public static UIElementMetadata PrintBargainAction(this UIElementMetadataBuilder elementMetadata)
        {
            return UIElementMetadata.Config
                                    .Name.Static("PrintBargainAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintBargain")
                                    .Operation.SpecificFor<PrintIdentity, Bargain>();
        }

        public static UIElementMetadata PrintNewSalesModelBargainAction(this UIElementMetadataBuilder elementMetadata)
        {
            return UIElementMetadata.Config
                                    .Name.Static("PrintNewSalesModelBargainAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintNewSalesModelBargain")
                                    .Operation.SpecificFor<PrintIdentity, Bargain>();
        }

        public static UIElementMetadata PrintBargainProlongationAgreementAction(this UIElementMetadataBuilder elementMetadata)
        {
            return UIElementMetadata.Config
                                    .Name.Static("PrintBargainProlongationAgreementAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainProlongationAgreementAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintBargainProlongationAgreement");
        }

        public static UIElementMetadata MergeLegalPersonsAction(this UIElementMetadataBuilder elementMetadata)
        {
            return UIElementMetadata.Config
                                    .Name.Static("Merge")
                                    .Icon.Path("Merge.gif")
                                    .Title.Resource(() => ErmConfigLocalization.ControlMerge)
                                    .ControlType(ControlType.ImageButton)
                                    .LockOnInactive()
                                    .LockOnNew()
                                    .Handler.Name("scope.Merge")
                                    .AccessWithPrivelege(FunctionalPrivilegeName.MergeLegalPersons)
                                    .Operation
                                    .SpecificFor<MergeIdentity, LegalPerson>();
        }

        public static UIElementMetadata[] With(this UIElementMetadata[] elements, params UIElementMetadata[] additionalElements)
        {
            return elements.Concat(additionalElements).ToArray();
        }
    }
}