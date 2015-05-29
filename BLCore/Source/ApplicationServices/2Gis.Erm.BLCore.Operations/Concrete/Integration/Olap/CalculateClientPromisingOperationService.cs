using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Olap;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Olap
{
    public class CalculateClientPromisingOperationService : ICalculateClientPromisingOperationService
    {
        private readonly IClientService _clientRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserContext _userContext;

        public CalculateClientPromisingOperationService(IClientService clientRepository, IOperationScopeFactory scopeFactory, IUserContext userContext)
        {
            _clientRepository = clientRepository;
            _scopeFactory = scopeFactory;
            _userContext = userContext;
        }

        public void CalculateClientPromising()
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CalculateClientPromisingIdentity>())
            {
                _clientRepository.CalculatePromising(_userContext.Identity.Code);

                scope.Complete();
            }
        }
    }
}