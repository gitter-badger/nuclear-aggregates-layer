using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel
{
    public interface IContributionTypeReadModel : ISimplifiedModelConsumerReadModel
    {
        string GetContributionTypeName(long bargainTypeId);
    }
}