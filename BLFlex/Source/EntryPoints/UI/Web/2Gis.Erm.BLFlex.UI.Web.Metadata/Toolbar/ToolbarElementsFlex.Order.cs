using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar
{
    public sealed partial class ToolbarElementsFlex
    {
        public static class Orders
        {
            public static UIElementMetadataBuilder ChangeDeal()
            {
                return
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
                                     .Operation.NonCoupled<ChangeDealIdentity>();
            }

            public static UIElementMetadataBuilder CheckOrder()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("CheckOrder")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCheckOrder)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnNew()
                                     .Handler.Name("scope.CheckOrder")
                                     .Operation.NonCoupled<ValidateOrdersIdentity>();
            }

            public static UIElementMetadataBuilder CloseWithDenial()
            {
                return
                    UIElementMetadata.Config
                                     .Name.Static("CloseWithDenial")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCloseWithDenial)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnNew()
                                     .Handler.Name("scope.CloseWithDenial")

                                     // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                     .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                     .Operation.NonCoupled<CloseWithDenialIdentity>();
            }

            public static UIElementMetadataBuilder SwitchToAccount()
            {
                return

                    // COMMENT {all, 01.12.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("SwitchToAccount")
                                     .Title.Resource(() => ErmConfigLocalization.ControlSwitchToAccount)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Handler.Name("scope.SwitchToAccount");
            }

            public static UIElementMetadataBuilder CopyOrder()
            {
                return

                    UIElementMetadata.Config
                                     .Name.Static("CopyOrder")
                                     .Title.Resource(() => ErmConfigLocalization.ControlCopyOrder)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnNew()
                                     .Handler.Name("scope.CopyOrder")
                                     .AccessWithPrivelege<Order>(EntityAccessTypes.Create)
                                     .AccessWithPrivelege<Order>(EntityAccessTypes.Update)
                                     .Operation.NonCoupled<CopyOrderIdentity>();
            }

            public static class Print
            {
                public static UIElementMetadataBuilder Additional(params UIElementMetadata[] printActions)
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintActionsAdditional")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintActionsAdditional)
                                         .ControlType(ControlType.Menu)
                                         .Childs(printActions);
                }

                public static UIElementMetadataBuilder Order()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintOrderAction")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintOrderAction)
                                         .ControlType(ControlType.TextButton)
                                         .LockOnNew()
                                         .Handler.Name("scope.PrintOrder")
                                         .Operation.SpecificFor<PrintIdentity, Order>();
                }

                public static UIElementMetadataBuilder OrderBargain()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintBargainAction")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                         .ControlType(ControlType.TextButton)
                                         .Handler.Name("scope.PrintOrderBargain")
                                         .Operation.SpecificFor<PrintIdentity, Bargain>();
                }

                public static UIElementMetadataBuilder Bill()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintBillAction")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintBillsAction)
                                         .ControlType(ControlType.TextButton)
                                         .Handler.Name("scope.PrintOrderBills")
                                         .Operation.SpecificFor<PrintIdentity, Bill>();
                }

                public static UIElementMetadataBuilder TerminationNotice()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintTerminationNoticeAction")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeAction)
                                         .ControlType(ControlType.TextButton)
                                         .Handler.Name("scope.PrintTerminationNotice");
                }

                public static UIElementMetadataBuilder AdditionalAgreement()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintAdditionalAgreementAction")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintAdditAgreementAction)
                                         .ControlType(ControlType.TextButton)
                                         .Handler.Name("scope.PrintAdditionalAgreement");
                }

                public static UIElementMetadataBuilder LetterOfGuarantee()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintLetterOfGuarantee")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintLetterOfGuaranteeAction)
                                         .ControlType(ControlType.TextButton)
                                         .Handler.Name("scope.PrintLetterOfGuarantee");
                }

                public static UIElementMetadataBuilder BargainAdditionalAgreement()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("PrintBargainAdditionalAgreementAction")
                                         .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAdditAgreementAction)
                                         .ControlType(ControlType.TextButton)
                                         .Handler.Name("scope.PrintOrderBargainAdditionalAgreement");
                }
            }

            public static class Czech
            {
                public static class Print
                {
                    public static UIElementMetadataBuilder SwornStatement()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrintLetterOfGuarantee")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrintSwornStatementAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrintLetterOfGuarantee");
                    }

                    public static UIElementMetadataBuilder TerminationNoticeWithoutReason()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrintTerminationNoticeWithoutReasonAction")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrintTermNoticeWithoutReasonAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrintTerminationNoticeWithoutReason");
                    }

                    public static UIElementMetadataBuilder TerminationBargainNotice()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrintTerminationBargainNoticeAction")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrintTermBargainNoticeAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrintTerminationBargainNotice");
                    }

                    public static UIElementMetadataBuilder TerminationBargainNoticeWithoutReason()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrintTerminationBargainNoticeWithoutReasonAction")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrintTermBargainNoticeWithoutReasonAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrintTerminationBargainNoticeWithoutReason");
                    }
                }
            }

            public static class Chile
            {
                public static class Print
                {
                    // COMMENT {all, 25.12.2014}: В Чехии есть похожая кнопка. возможно, можно будет объединить.
                    public static UIElementMetadataBuilder TerminationBargainNotice()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrintTerminationNoticeAction")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrintTerminationBargainNoticeAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrintTerminationNotice");
                    }
                }
            }

            public static class Russia
            {
                public static class Print
                {
                    public static UIElementMetadataBuilder NewSalesModelBargain()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrintNewSalesModelBargainAction")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrintNewSalesModelOrderBargain")
                                             .Operation.SpecificFor<PrintIdentity, Bargain>();
                    }

                    public static UIElementMetadataBuilder JointBill()
                    {
                        return
                            UIElementMetadata.Config
                                             .Name.Static("PrepareJointBillsAction")
                                             .Title.Resource(() => ErmConfigLocalization.ControlPrepareJointBillsAction)
                                             .ControlType(ControlType.TextButton)
                                             .Handler.Name("scope.PrepareJointBill");
                    }
                }
            }
        }
    }
}
