using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditCurrencyRateHandler : RequestHandler<EditRequest<CurrencyRate>, EmptyResponse>
    {
        private readonly ICurrencyService _currencyService;

        public EditCurrencyRateHandler(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override EmptyResponse Handle(EditRequest<CurrencyRate> request)
        {
            var currencyRate = request.Entity;
            _currencyService.SetCurrencyRate(currencyRate);
            return Response.Empty;
        }
    }
}
