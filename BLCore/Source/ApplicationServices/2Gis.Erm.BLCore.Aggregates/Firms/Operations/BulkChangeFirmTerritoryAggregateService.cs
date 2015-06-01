using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkChangeFirmTerritoryAggregateService : IBulkChangeFirmTerritoryAggregateService
    {
        private readonly IRepository<Firm> _firmRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkChangeFirmTerritoryAggregateService(IRepository<Firm> firmRepository, IOperationScopeFactory scopeFactory)
        {
            _firmRepository = firmRepository;
            _scopeFactory = scopeFactory;
        }

        public void ChangeTerritory(IEnumerable<ChangeFirmTerritoryDto> firmTerritories)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ChangeTerritoryIdentity, Firm>())
            {
                foreach (var firmTerritory in firmTerritories)
                {
                    var firm = firmTerritory.Firm;
                    if (firm.TerritoryId == firmTerritory.TerritoryId)
                    {
                        continue;
                    }

                    firm.TerritoryId = firmTerritory.TerritoryId;
                    _firmRepository.Update(firm);
                    scope.Updated(firm);
                }

                _firmRepository.Save();
                scope.Complete();
            }
        }
    }
}