using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel
{
    public interface IContributionTypeReadModel : ISimplifiedModelConsumerReadModel
    {
        string GetContributionTypeName(long bargainTypeId);
    }
}