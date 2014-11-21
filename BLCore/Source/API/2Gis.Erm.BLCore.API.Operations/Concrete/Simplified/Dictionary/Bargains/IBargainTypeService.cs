using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Bargains
{
    public interface IBargainTypeService : ISimplifiedModelConsumer
    {
        void CreateOrUpdate(BargainType bargainType);
    }
}
