using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic
{
    public interface IOrderPrintFormDataExtractor
    {
        PrintData GetPaymentSchedule(IQueryable<Bill> query);
        PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile);
        PrintData GetOrder(IQueryable<Order> query);
        PrintData GetBargain(Bargain bargain);
        PrintData GetBranchOfficeData(BranchOffice branchOffice);
        PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou);
        PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery);
        PrintData GetLegalPersonData(LegalPerson legalPerson);
        PrintData GetUngrouppedFields(IQueryable<Order> query);
    }
}