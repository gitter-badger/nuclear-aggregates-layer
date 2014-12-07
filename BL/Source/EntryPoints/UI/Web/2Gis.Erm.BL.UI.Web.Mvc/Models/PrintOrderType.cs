using System;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public enum PrintOrderType
    {
        /// <summary>
        /// Заказ
        /// </summary>
        PrintOrder,
        [Obsolete] PrintRegionalOrder,
        PrintBargain,
        PrintSingleBill,
        PrintOrderBills,
        PrintTerminationNotice,
        PrintTerminationNoticeWithoutReason,
        PrintTerminationBargainNotice,
        PrintTerminationBargainNoticeWithoutReason,
        [Obsolete] PrintRegionalTerminationNotice,
        PrintAdditionalAgreement,
        PrintBargainAdditionalAgreement,
        [Obsolete] PrintReferenceInformation,
        PrepareJointBill,
        PrintLetterOfGuarantee,
        PrintNewSalesModelBargain,
    }
}