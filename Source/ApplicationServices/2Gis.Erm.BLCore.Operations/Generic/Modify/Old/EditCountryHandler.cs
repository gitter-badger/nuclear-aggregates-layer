using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Countries;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditCountryHandler : RequestHandler<EditRequest<Country>, EmptyResponse>
    {
        private readonly ICountryService _countryService;

        public EditCountryHandler(ICountryService countryService)
        {
            _countryService = countryService;
        }

        protected override EmptyResponse Handle(EditRequest<Country> request)
        {
            _countryService.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}