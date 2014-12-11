using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc
{
    public static class UiMetadataExtensions
    {
        public static UiElementMetadata[] OrderAdditionalActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                            .Operation.NonCoupled<CopyOrderIdentity>()
                       };
        }

        public static UiElementMetadata[] CyprusOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"))
                       };
        }

        public static UiElementMetadata[] CzechOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeWithoutReasonAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeWithoutReasonAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNoticeWithoutReason"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintTerminationBargainNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermBargainNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationBargainNotice"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintTerminationBargainNoticeWithoutReasonAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTermBargainNoticeWithoutReasonAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationBargainNoticeWithoutReason"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintBargainAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargainAdditionalAgreement"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintSwornStatementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UiElementMetadata[] RussianOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UiElementMetadata[] ChileOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintTerminationNoticeAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintTerminationBargainNoticeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintTerminationNotice"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintLetterOfGuarantee")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UiElementMetadata[] UkraineOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
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
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UiElementMetadata[] EmiratesOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintAdditionalAgreement"),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintBargainAdditionalAgreementAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAdditAgreementAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargainAdditionalAgreement"))
                       };
        }

        public static UiElementMetadata[] KazakhstanOrderPrintActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           UiElementMetadata.Config
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
                                                                     .Name.Static("PrintBillAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBills")
                                                                     .Operation.SpecificFor<PrintIdentity, Bill>(),
                                                    UiElementMetadata.Config
                                                                     .Name.Static("PrintBargainAction")
                                                                     .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                     .ControlType(ControlType.TextButton)
                                                                     .Handler.Name("scope.PrintOrderBargain")
                                                                     .Operation.SpecificFor<PrintIdentity, Bargain>(),
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
                                                                     .Handler.Name("scope.PrintLetterOfGuarantee"))
                       };
        }

        public static UiElementMetadata[] CommonLegalPersonAdditionalActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                           // COMMENT {all, 29.11.2014}: а как же безопасность?
                           UiElementMetadata.Config
                                            .Name.Static("ChangeLegalPersonClient")
                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeLegalPersonClient)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnInactive()
                                            .LockOnNew()
                                            .Handler.Name("scope.ChangeLegalPersonClient")
                                            .Operation.SpecificFor<ChangeClientIdentity, LegalPerson>(),

                           // COMMENT {all, 29.11.2014}: а как же безопасность?
                           UiElementMetadata.Config
                                            .Name.Static("ChangeLPRequisites")
                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeLPRequisites)
                                            .ControlType(ControlType.TextButton)
                                            .LockOnInactive()
                                            .LockOnNew()
                                            .Handler.Name("scope.ChangeLegalPersonRequisites")
                                            .Operation.NonCoupled<ChangeRequisitesIdentity>()
                       };
        }

        public static UiElementMetadata[] CommonOrderRelatedActions(this UiElementMetadataBuilder elementMetadata)
        {
            return new UiElementMetadata[]
                       {
                          UiElementMetadata.Config.ContentTab(),
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
                                                             .FilterToParent()
                       };
        }

        public static UiElementMetadata PrintBargainAction(this UiElementMetadataBuilder elementMetadata)
        {
            return UiElementMetadata.Config
                                    .Name.Static("PrintBargainAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintBargain")
                                    .Operation.SpecificFor<PrintIdentity, Bargain>();
        }

        public static UiElementMetadata PrintNewSalesModelBargainAction(this UiElementMetadataBuilder elementMetadata)
        {
            return UiElementMetadata.Config
                                    .Name.Static("PrintNewSalesModelBargainAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintNewSalesModelBargain")
                                    .Operation.SpecificFor<PrintIdentity, Bargain>();
        }

        public static UiElementMetadata PrintBargainProlongationAgreementAction(this UiElementMetadataBuilder elementMetadata)
        {
            return UiElementMetadata.Config
                                    .Name.Static("PrintBargainProlongationAgreementAction")
                                    .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainProlongationAgreementAction)
                                    .ControlType(ControlType.TextButton)
                                    .LockOnNew()
                                    .Handler.Name("scope.PrintBargainProlongationAgreement");
        }

        public static UiElementMetadata MergeLegalPersonsAction(this UiElementMetadataBuilder elementMetadata)
        {
            return UiElementMetadata.Config
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

        public static UiElementMetadata[] With(this UiElementMetadata[] elements, params UiElementMetadata[] additionalElements)
        {
            return elements.Concat(additionalElements).ToArray();
        }
    }
}