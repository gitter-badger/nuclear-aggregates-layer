namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public enum PrintOrderType
    {
        /// <summary>
        /// Заказ
        /// </summary>
        PrintOrder,
        PrintRegionalOrder,
        PrintBargain,
        PrintSingleBill,
        PrintOrderBills,
        PrintTerminationNotice,
        PrintTerminationNoticeWithoutReason,
        PrintTerminationBargainNotice,
        PrintTerminationBargainNoticeWithoutReason,
        PrintRegionalTerminationNotice,
        PrintAdditionalAgreement,
        PrintBargainAdditionalAgreement,
        PrintReferenceInformation,
        PrepareJointBill,
        PrintLetterOfGuarantee,
        PrintNewSalesModelBargain,
    }
}
