using System.Linq;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic
{
    public interface IBargainPrintFormDataExtractor
    {
        PrintData GetBranchOffice(IQueryable<BranchOffice> query);
        PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou);
        PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile);
        PrintData GetLegalPerson(LegalPerson legalPerson);
        PrintData GetUngroupedFields(IQueryable<Bargain> bargainQuery);
        PrintData GetBargain(IQueryable<Bargain> queryable);
    }
}