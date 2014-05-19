using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel
{
    public interface IBargainTypeReadModel : ISimplifiedModelConsumerReadModel
    {
        decimal GetVatRate(long bargainTypeId);
    }
}
