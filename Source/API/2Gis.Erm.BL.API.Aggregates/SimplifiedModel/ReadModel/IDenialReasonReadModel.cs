using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel
{
    public interface IDenialReasonReadModel : ISimplifiedModelConsumerReadModel
    {
        DenialReason GetDenialReason(long denialReasonId);
        bool IsThereDuplicateByName(long denialReasonId, string name);
    }
}