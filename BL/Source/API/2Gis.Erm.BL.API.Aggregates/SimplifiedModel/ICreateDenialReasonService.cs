using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel
{
    public interface ICreateDenialReasonService : ISimplifiedModelConsumer
    {
        int Create(DenialReason denialReason);
    }
}
