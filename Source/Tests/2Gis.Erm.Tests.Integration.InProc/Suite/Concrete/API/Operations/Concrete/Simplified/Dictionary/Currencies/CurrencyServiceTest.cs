using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Currencies
{
    public class CurrencyServiceTest : UseModelEntityTestBase<Currency>
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyServiceTest(IAppropriateEntityProvider<Currency> appropriateEntityProvider, ICurrencyService currencyService) : base(appropriateEntityProvider)
        {
            _currencyService = currencyService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Currency modelEntity)
        {
            _currencyService.GetCurrencyWithRelations(modelEntity.Id);

            var currencyRate = new CurrencyRate { CurrencyId = modelEntity.Id, BaseCurrencyId = modelEntity.Id };
            _currencyService.SetCurrencyRate(currencyRate);

            modelEntity.Name = "Test";
            _currencyService.CreateOrUpdate(modelEntity);

            _currencyService.Delete(modelEntity);


            return Result.When(currencyRate)
                         .Then(c => c.Id.Should().NotBe(0));
        }
    }
}