using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Currencies;
using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Currencies
{
    public sealed class CurrencyReadModel : ICurrencyReadModel
    {
        private readonly IFinder _finder;

        public CurrencyReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Currency GetCurrency(long id)
        {
            return _finder.FindOne(Specs.Find.ById<Currency>(id));
        }
    }
}
