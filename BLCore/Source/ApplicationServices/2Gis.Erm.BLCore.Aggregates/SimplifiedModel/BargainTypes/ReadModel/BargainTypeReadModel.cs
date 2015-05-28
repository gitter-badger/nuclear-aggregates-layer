using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.BargainTypes.ReadModel
{
    public class BargainTypeReadModel : IBargainTypeReadModel
    {
        private readonly ISecureFinder _finder;

        public BargainTypeReadModel(ISecureFinder finder)
        {
            _finder = finder;
        }

        public decimal GetVatRate(long bargainTypeId)
        {
            return _finder.FindObsolete(Specs.Find.ById<BargainType>(bargainTypeId)).Single().VatRate;
        }

        public string GetBargainTypeName(long bargainTypeId)
        {
            return _finder.FindObsolete(Specs.Find.ById<BargainType>(bargainTypeId)).Select(x => x.Name).Single();
        }
    }
}
