using DoubleGis.Erm.BLCore.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Olap
{
    public sealed class CalculateClientPromisingHandler : RequestHandler<CalculateClientPromisingRequest, EmptyResponse>
    {
        private readonly IClientRepository _clientRepository;

        public CalculateClientPromisingHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        protected override EmptyResponse Handle(CalculateClientPromisingRequest request)
        {
            _clientRepository.CalculatePromising();
            return Response.Empty;
        }
    }
}