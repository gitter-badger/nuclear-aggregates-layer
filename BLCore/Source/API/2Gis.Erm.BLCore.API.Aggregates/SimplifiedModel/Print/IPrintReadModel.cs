using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Print
{
    public interface IPrintReadModel : ISimplifiedModelConsumerReadModel
    {
        Order GetOrder(long id);
        Currency GetCurrency(long? id);
        Bargain GetBargain(long? id);
        LegalPerson GetLegalPerson(long? id);
        LegalPersonProfile GetLegalPersonProfile(long? id);
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long? id);
        BranchOffice GetBranchOffice(long id);
        Firm GetFirm(long? id);
    }
}
