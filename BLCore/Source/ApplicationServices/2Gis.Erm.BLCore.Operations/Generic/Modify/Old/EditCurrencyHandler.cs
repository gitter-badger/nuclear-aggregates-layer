using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditCurrencyHandler : RequestHandler<EditRequest<Currency>, EmptyResponse>
    {
        private readonly ICurrencyService _currencyService;

        public EditCurrencyHandler(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override EmptyResponse Handle(EditRequest<Currency> request)
        {
            var currency = request.Entity;
            _currencyService.CreateOrUpdate(currency);
            return Response.Empty;
        }
   }
}