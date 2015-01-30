using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic
{
    public interface IOrderPrintFormDataExtractor
    {
        PrintData GetPaymentSchedule(IQueryable<Bill> query);
        PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile);
        PrintData GetBranchOffice(IQueryable<BranchOffice> query);
        PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou);
        PrintData GetOrder(IQueryable<Order> query);
        PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> query, SalesModel? salesModel);
        PrintData GetUngrouppedFields(IQueryable<Order> query, BranchOfficeOrganizationUnit branchOfficeOrganizationUnit, LegalPerson legalPerson,  LegalPersonProfile legalPersonProfile);
    }
}