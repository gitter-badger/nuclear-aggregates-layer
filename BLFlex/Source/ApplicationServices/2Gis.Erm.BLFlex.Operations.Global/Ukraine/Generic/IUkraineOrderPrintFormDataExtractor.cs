using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic
{
    public interface IUkraineOrderPrintFormDataExtractor
    {
        PrintData GetPaymentSchedule(IQueryable<Bill> query);
        PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile);
        PrintData GetOrder(IQueryable<Order> query);
        PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts);
        PrintData GetBranchOfficeData(BranchOffice branchOffice);
        PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou);
        PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery);
        PrintData GetLegalPersonData(LegalPerson legalPerson);
        PrintData GetUngrouppedFields(IQueryable<Order> query);
    }
}