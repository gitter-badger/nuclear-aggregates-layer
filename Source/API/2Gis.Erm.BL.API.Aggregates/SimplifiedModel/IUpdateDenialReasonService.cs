using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel
{
    public interface IUpdateDenialReasonService : ISimplifiedModelConsumer
    {
        int Update(DenialReason denialReason);
    }
}