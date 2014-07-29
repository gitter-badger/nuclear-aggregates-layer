using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies
{
    public interface ICurrencyService : ISimplifiedModelConsumer
    {
        void Delete(Currency currency);
        CurrencyWithRelationsDto GetCurrencyWithRelations(long entityId);
        void CreateOrUpdate(Currency currency);

        void SetCurrencyRate(CurrencyRate currencyRate);
    }
}