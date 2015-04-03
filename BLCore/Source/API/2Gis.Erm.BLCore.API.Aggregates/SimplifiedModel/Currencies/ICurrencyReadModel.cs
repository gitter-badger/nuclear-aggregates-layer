using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Currencies
{
    public interface ICurrencyReadModel : ISimplifiedModelConsumerReadModel
    {
        Currency GetCurrency(long id);
    }
}
