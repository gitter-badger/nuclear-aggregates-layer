using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class GetOrderLegalPersonHandler : RequestHandler<GetOrderLegalPersonRequest, GetOrderLegalPersonResponse>
    {
        private readonly ILegalPersonRepository _legalPersonRepository;

        public GetOrderLegalPersonHandler(ILegalPersonRepository legalPersonRepository)
        {
            _legalPersonRepository = legalPersonRepository;
        }

        protected override GetOrderLegalPersonResponse Handle(GetOrderLegalPersonRequest request)
        {
            var response = new GetOrderLegalPersonResponse { LegalPersonId = null, LegalPersonName = null };
            if (request.FirmClientId == 0)
            {
                return response;
            }

            var name = _legalPersonRepository.GetLegalPersonNameByClientId(request.FirmClientId);
            if (name == null)
            {
                return response;
            }

            response.LegalPersonId = name.Id;
            response.LegalPersonName = name.Name;
            return response;
        }
    }
}