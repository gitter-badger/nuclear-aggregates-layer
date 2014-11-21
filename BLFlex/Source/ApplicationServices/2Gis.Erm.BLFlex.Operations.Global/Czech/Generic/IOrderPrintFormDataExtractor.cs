using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic
{
    public interface IOrderPrintFormDataExtractor
    {
        PrintData GetPaymentSchedule(IQueryable<Bill> query);
        PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile);
        PrintData GetOrder(IQueryable<Order> query);
        PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts);
        PrintData GetBranchOffice(IQueryable<BranchOffice> query);
        PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou);
        PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery);
        PrintData GetLegalPerson(LegalPerson legalPerson);
        PrintData GetUngrouppedFields(IQueryable<Order> query);
        PrintData GetClient(LegalPerson legalPerson, LegalPersonProfile legalPersonProfile);
    }
}