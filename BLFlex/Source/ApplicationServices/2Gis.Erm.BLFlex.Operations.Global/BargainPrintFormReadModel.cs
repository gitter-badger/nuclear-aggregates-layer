using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public class BargainPrintFormReadModel : IBargainPrintFormReadModel
    {
        private readonly IFinder _finder;

        public BargainPrintFormReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public BargainRelationsDto GetBargainRelationsDto(long bargainId)
        {
            return _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                          .Select(x => new BargainRelationsDto
                              {
                                  BargainNumber = x.Number,
                                  BargainKind = x.BargainKind,
                                  BranchOfficeOrganizationUnitId = x.ExecutorBranchOfficeId,
                                  LegalPersonId = x.CustomerLegalPersonId
                              })
                          .Single();
        }

        public IQueryable<Bargain> GetBargainQuery(long bargainId)
        {
            return _finder.Find(Specs.Find.ById<Bargain>(bargainId));
        }

        public IQueryable<BranchOffice> GetBranchOfficeQuery(long bargainId)
        {
            // COMMENT {all, 13.05.2014}: Тут нормально, поскольку этот BranchOffice не будет вытянут целиком
            // COMMENT {a.rechkalov, 21.05.2014}: Отдавая наружу IQueryable нельзя быть в этом уверенным
            return _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                          .Select(x => x.BranchOfficeOrganizationUnit.BranchOffice);
        }
    }
}