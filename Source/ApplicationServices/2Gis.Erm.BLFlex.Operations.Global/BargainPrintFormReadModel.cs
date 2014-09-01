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
                                  LegalPersonProfileId = order.LegalPersonProfileId,
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
            // COMMENT {all, 13.05.2014}: Тут нормально, поскольку этот BranchOffice не будет вытянут целиком
            // COMMENT {a.rechkalov, 21.05.2014}: Отдавая наружу IQueryable нельзя быть в этом уверенным
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => order.BranchOfficeOrganizationUnit.BranchOffice);
        }

        public IQueryable<Order> GetOrderQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId));
        }
    }
}