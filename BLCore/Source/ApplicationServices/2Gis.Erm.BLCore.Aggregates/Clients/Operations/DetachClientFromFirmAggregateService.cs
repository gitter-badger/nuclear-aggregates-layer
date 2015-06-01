using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class DetachClientFromFirmAggregateService : IDetachClientFromFirmAggregateService
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DetachClientFromFirmAggregateService(IRepository<Client> clientRepository, IOperationScopeFactory scopeFactory)
        {
            _clientRepository = clientRepository;
            _scopeFactory = scopeFactory;
        }

        public void Detach(IEnumerable<Client> clientsToDetach)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DetachIdentity, Client, Firm>())
            {
                foreach (var client in clientsToDetach)
                {
                    client.MainFirmId = null;
                    _clientRepository.Update(client);
                    scope.Updated(client);
                }

                _clientRepository.Save();
                scope.Complete();
            }
        }
    }
}