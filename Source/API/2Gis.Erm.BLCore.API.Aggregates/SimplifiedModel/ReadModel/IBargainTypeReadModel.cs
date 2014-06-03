using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel
{
    public interface IBargainTypeReadModel : ISimplifiedModelConsumerReadModel
    {
        decimal GetVatRate(long bargainTypeId);

        string GetBargainTypeName(long bargainTypeId);
    }
}
