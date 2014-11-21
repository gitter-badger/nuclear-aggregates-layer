using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic
{
    public interface IOrderPrintFormDataExtractor
    {
        PrintData GetPaymentSchedule(IQueryable<Bill> query);
        PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile, ChileLegalPersonProfilePart legalPersonProfilePart, Bank bank);
        PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> query);
        PrintData GetLegalPerson(LegalPerson legalPerson);
        PrintData GetOrder(IQueryable<Order> query);
        PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts);
        PrintData GetBranchOffice(IQueryable<BranchOffice> query);
        PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou, ChileBranchOfficeOrganizationUnitPart boouPart);
        PrintData GetUngrouppedFields(IQueryable<Order> query);
    }
}