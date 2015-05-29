using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeTerritory
{
    public class ChangeClientTerritoryService : IChangeGenericEntityTerritoryService<Client>
    {
        private readonly IClientService _clientRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IClientReadModel _clientReadModel;

        public ChangeClientTerritoryService(IClientService clientRepository, IOperationScopeFactory operationScopeFactory, IClientReadModel clientReadModel)
        {
            _clientRepository = clientRepository;
            _operationScopeFactory = operationScopeFactory;
            _clientReadModel = clientReadModel;
        }

        public void ChangeTerritory(long entityId, long territoryId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Client>())
            {
                // TODO: Refactor for UserRepository usage
                // _publicService.Handle(new ValidateTerritoryAvailabilityRequest { TerritoryId = territoryId });

                var client = _clientReadModel.GetClient(entityId);
                var changeAggregateTerritoryRepository = _clientRepository as IChangeAggregateTerritoryRepository<Client>;
                changeAggregateTerritoryRepository.ChangeTerritory(entityId, territoryId);

                operationScope.Updated<Client>(entityId);
                operationScope.Updated<Territory>(territoryId, client.TerritoryId);
                operationScope.Complete();
            }
        }
    }
}