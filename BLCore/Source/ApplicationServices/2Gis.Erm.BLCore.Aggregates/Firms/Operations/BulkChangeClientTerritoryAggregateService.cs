using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkChangeClientTerritoryAggregateService : IBulkChangeClientTerritoryAggregateService
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkChangeClientTerritoryAggregateService(IRepository<Client> clientRepository, IOperationScopeFactory scopeFactory)
        {
            _clientRepository = clientRepository;
            _scopeFactory = scopeFactory;
        }

        public void ChangeTerritory(IEnumerable<ChangeClientTerritoryDto> clientTerritories)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Client>())
            {
                foreach (var clientTerritory in clientTerritories)
                {
                    var client = clientTerritory.Client;
                    if (client.TerritoryId == clientTerritory.TerritoryId)
                    {
                        continue;
                    }

                    client.TerritoryId = clientTerritory.TerritoryId;
                    _clientRepository.Update(client);
                    scope.Updated(client);
                }

                _clientRepository.Save();
                scope.Complete();
            }
        }
    }
}