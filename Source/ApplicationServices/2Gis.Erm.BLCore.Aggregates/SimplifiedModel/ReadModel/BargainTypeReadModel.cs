using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel
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
            return _finder.Find(Specs.Find.ById<BargainType>(bargainTypeId)).Single().VatRate;
        }
    }
}
