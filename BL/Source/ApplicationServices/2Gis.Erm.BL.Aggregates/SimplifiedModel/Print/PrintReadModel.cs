using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Print;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Aggregates.SimplifiedModel.Print
{
    public sealed class PrintReadModel : IPrintReadModel
    {
        private readonly IFinder _finder;

        public PrintReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Order GetOrder(long id)
        {
            return _finder.FindOne(Specs.Find.ById<Order>(id));
        }

        public Currency GetCurrency(long? id)
        {
            return _finder.FindOne(Specs.Find.ById<Currency>(id));
        }

        public Bargain GetBargain(long? id)
        {
            return _finder.FindOne(Specs.Find.ById<Bargain>(id));
        }

        public LegalPerson GetLegalPerson(long? id)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPerson>(id));
        }

        public LegalPersonProfile GetLegalPersonProfile(long? id)
        {
            return _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(id));
        }

        public BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long? id)
        {
            return _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(id));
        }

        public BranchOffice GetBranchOffice(long id)
        {
            return _finder.FindOne(Specs.Find.ById<BranchOffice>(id));
        }

        public Firm GetFirm(long? id)
        {
            return _finder.FindOne(Specs.Find.ById<Firm>(id));
        }
    }
}
