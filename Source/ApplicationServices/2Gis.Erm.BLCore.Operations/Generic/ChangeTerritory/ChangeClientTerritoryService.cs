using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeTerritory
{
    public class ChangeClientTerritoryService : IChangeGenericEntityTerritoryService<Client>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ChangeClientTerritoryService(IClientRepository clientRepository, IOperationScopeFactory operationScopeFactory)
        {
            _clientRepository = clientRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void ChangeTerritory(long entityId, long territoryId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Client>())
            {
                // TODO: Refactor for UserRepository usage
                // _publicService.Handle(new ValidateTerritoryAvailabilityRequest { TerritoryId = territoryId });

                var client = _clientRepository.GetClient(entityId);
                var changeAggregateTerritoryRepository = _clientRepository as IChangeAggregateTerritoryRepository<Client>;
                changeAggregateTerritoryRepository.ChangeTerritory(entityId, territoryId);

                operationScope.Updated<Client>(entityId);
                operationScope.Updated<Territory>(territoryId, client.TerritoryId);
                operationScope.Complete();
            }
        }
    }
}