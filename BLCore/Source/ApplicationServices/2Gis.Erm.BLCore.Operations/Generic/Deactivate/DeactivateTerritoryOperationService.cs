using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateTerritoryOperationService : IDeactivateGenericEntityService<Territory>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFirmRepository _firmRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeactivateTerritoryOperationService(IUserRepository userRepository,
                                                   IFirmRepository firmRepository,
                                                   IClientRepository clientRepository,
                                                   IOperationScopeFactory operationScopeFactory)
        {
            _userRepository = userRepository;
            _firmRepository = firmRepository;
            _clientRepository = clientRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeactivateConfirmation Deactivate(long deactivateTerritoryId, long newTerritoryId)
        {
            if (!_firmRepository.IsTerritoryReplaceable(deactivateTerritoryId, newTerritoryId))
            {
                throw new ArgumentException(BLResources.CannotReplaceTerritories);
            }

            var territory = _userRepository.GetTerritory(newTerritoryId);
            if (territory == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound, "newTerritoryId");
            }

            using (var scope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, Territory>())
            {
                var firms = _firmRepository.GetFirmsByTerritory(deactivateTerritoryId);
                _firmRepository.ChangeTerritory(firms, newTerritoryId);

                var clients = _clientRepository.GetClientsByTerritory(deactivateTerritoryId);
                _clientRepository.ChangeTerritory(clients, newTerritoryId);

                var users = _userRepository.GetUsersByTerritory(deactivateTerritoryId);
                _userRepository.ChangeUserTerritory(users, deactivateTerritoryId, newTerritoryId);

                var deactivateAggregateRepository = _userRepository as IDeactivateAggregateRepository<Territory>;
                deactivateAggregateRepository.Deactivate(deactivateTerritoryId);

                scope.Updated<Territory>(deactivateTerritoryId)
                     .Complete();
            }

            return null;
        }
    }
}