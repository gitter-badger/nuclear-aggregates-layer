using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc
{
    public static class UIMetadataExtensions
    {
        public static UIElementMetadata[] OrderAdditionalActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.ChangeProfiles(),
                           ToolbarElementsFlex.Orders.ChangeDeal(),
                           ToolbarElementsFlex.Orders.CheckOrder(),
                           ToolbarElements.ChangeOwner<Order>(),
                           ToolbarElementsFlex.Orders.CloseWithDenial(),
                           ToolbarElementsFlex.Orders.SwitchToAccount(),
                           ToolbarElementsFlex.Orders.CopyOrder()
                       };
        }

        public static UIElementMetadata[] CyprusOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.Print.Order(),
                           ToolbarElementsFlex.Orders.Print.Additional(ToolbarElementsFlex.Orders.Print.OrderBargain(),
                                                                       ToolbarElementsFlex.Orders.Print.TerminationNotice(),
                                                                       ToolbarElementsFlex.Orders.Print.AdditionalAgreement())
                       };
        }

        public static UIElementMetadata[] CzechOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.Print.Order(),
                           ToolbarElementsFlex.Orders.Print.Additional(ToolbarElementsFlex.Orders.Print.Bill(),
                                                                       ToolbarElementsFlex.Orders.Print.OrderBargain(),
                                                                       ToolbarElementsFlex.Orders.Print.TerminationNotice(),
                                                                       ToolbarElementsFlex.Orders.Czech.Print.TerminationNoticeWithoutReason(),
                                                                       ToolbarElementsFlex.Orders.Czech.Print.TerminationBargainNotice(),
                                                                       ToolbarElementsFlex.Orders.Czech.Print.TerminationBargainNoticeWithoutReason(),
                                                                       ToolbarElementsFlex.Orders.Print.AdditionalAgreement(),
                                                                       ToolbarElementsFlex.Orders.Print.BargainAdditionalAgreement(),
                                                                       ToolbarElementsFlex.Orders.Czech.Print.SwornStatement())
                       };
        }

        public static UIElementMetadata[] RussianOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.Print.Order(),
                           ToolbarElementsFlex.Orders.Print.Additional(ToolbarElementsFlex.Orders.Print.OrderBargain(),
                                                                       ToolbarElementsFlex.Orders.Russia.Print.NewSalesModelBargain(),
                                                                       ToolbarElementsFlex.Orders.Russia.Print.JointBill(),
                                                                       ToolbarElementsFlex.Orders.Print.Bill(),
                                                                       ToolbarElementsFlex.Orders.Print.TerminationNotice(),
                                                                       ToolbarElementsFlex.Orders.Print.AdditionalAgreement(),
                                                                       ToolbarElementsFlex.Orders.Print.LetterOfGuarantee())
                       };
        }

        public static UIElementMetadata[] ChileOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.Print.Order(),
                           ToolbarElementsFlex.Orders.Print.Additional(ToolbarElementsFlex.Orders.Print.OrderBargain(),
                                                                       ToolbarElementsFlex.Orders.Print.Bill(),
                                                                       ToolbarElementsFlex.Orders.Chile.Print.TerminationBargainNotice(),
                                                                       ToolbarElementsFlex.Orders.Print.LetterOfGuarantee())
                       };
        }

        public static UIElementMetadata[] EmiratesOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.Print.Order(),
                           ToolbarElementsFlex.Orders.Print.Additional(ToolbarElementsFlex.Orders.Print.OrderBargain(),
                                                                       ToolbarElementsFlex.Orders.Print.Bill(),
                                                                       ToolbarElementsFlex.Orders.Print.AdditionalAgreement(),
                                                                       ToolbarElementsFlex.Orders.Print.BargainAdditionalAgreement())
                       };
        }

        public static UIElementMetadata[] UkraineAndKazakhstanOrderPrintActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.Orders.Print.Order(),
                           ToolbarElementsFlex.Orders.Print.Additional(ToolbarElementsFlex.Orders.Print.Bill(),
                                                                       ToolbarElementsFlex.Orders.Print.OrderBargain(),
                                                                       ToolbarElementsFlex.Orders.Print.TerminationNotice(),
                                                                       ToolbarElementsFlex.Orders.Print.AdditionalAgreement(),
                                                                       ToolbarElementsFlex.Orders.Print.LetterOfGuarantee())
                       };
        }

        public static UIElementMetadata[] CommonLegalPersonAdditionalActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           ToolbarElementsFlex.LegalPersons.ChangeClient(),
                           ToolbarElementsFlex.LegalPersons.ChangeRequisites()
                       };
        }

        public static UIElementMetadata[] CommonOrderRelatedActions(this UIElementMetadataBuilder elementMetadata)
        {
            return new UIElementMetadata[]
                       {
                           RelatedItem.ContentTab(),
                           RelatedItem.EntityGrid(EntityName.Bill, () => ErmConfigLocalization.CrdRelBills),
                           RelatedItem.EntityGrid(EntityName.Lock, () => ErmConfigLocalization.CrdRelLocks),
                           RelatedItem.EntityGrid(EntityName.OrderFile, () => ErmConfigLocalization.CrdRelOrderFiles)
                       };
        }

        public static UIElementMetadata[] With(this UIElementMetadata[] elements, params UIElementMetadata[] additionalElements)
        {
            return elements.Concat(additionalElements).ToArray();
        }
    }
}