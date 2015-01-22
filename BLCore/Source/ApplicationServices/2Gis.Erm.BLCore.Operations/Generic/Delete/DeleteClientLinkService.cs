using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteClientLinkService : IDeleteGenericEntityService<ClientLink>
    {
        private readonly IDeleteClientLinkAggregateService _deleteAggregateService;
        private readonly IClientReadModel _clientReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteClientLinkService(IDeleteClientLinkAggregateService deleteAggregateService, IClientReadModel clientReadModel, IOperationScopeFactory operationScopeFactory)
        {
            _deleteAggregateService = deleteAggregateService;
            _clientReadModel = clientReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, ClientLink>())
            {
                var clientLink = _clientReadModel.GetClientsLink(entityId);
                _deleteAggregateService.Delete(clientLink, _clientReadModel.GetCurrentDenormalizationForClientLink(clientLink.MasterClientId, clientLink.ChildClientId));
                scope.Deleted(clientLink);
                scope.Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}