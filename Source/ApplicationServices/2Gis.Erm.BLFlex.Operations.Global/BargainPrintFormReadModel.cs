using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public class BargainPrintFormReadModel : IBargainPrintFormReadModel
    {
        private readonly IFinder _finder;

        public BargainPrintFormReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public BargainRelationsDto GetBargainRelationsDto(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new BargainRelationsDto
                              {
                                  BargainNumber = order.Bargain.Number,
                                  CurrencyIsoCode = order.Currency.ISOCode,
                                  BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                                  MainLegalPersonProfileId = order.LegalPerson.LegalPersonProfiles.FirstOrDefault(y => y.IsMainProfile).Id,
                                  LegalPersonId = order.LegalPersonId
                              })
                          .Single();
        }

        public IQueryable<Bargain> GetBargainQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => order.Bargain);
        }

        public IQueryable<BranchOffice> GetBranchOfficeQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => order.BranchOfficeOrganizationUnit.BranchOffice);
        }

        public IQueryable<Order> GetOrderQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId));
        }
    }
}