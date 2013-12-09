using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Currency
{
    public class CurrencyServiceTest : UseModelEntityTestBase<Platform.Model.Entities.Erm.Currency>
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyServiceTest(IAppropriateEntityProvider<Platform.Model.Entities.Erm.Currency> appropriateEntityProvider, ICurrencyService currencyService) : base(appropriateEntityProvider)
        {
            _currencyService = currencyService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Platform.Model.Entities.Erm.Currency modelEntity)
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