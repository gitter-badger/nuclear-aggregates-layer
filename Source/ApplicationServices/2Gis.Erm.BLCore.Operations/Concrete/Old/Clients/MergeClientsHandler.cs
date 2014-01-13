using System;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.CrmActivities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Microsoft.Crm.SdkTypeProxy;

using Response = DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse.Response;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public sealed class MergeClientsHandler : RequestHandler<MergeClientsRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IClientRepository _clientRepository;
        private readonly ICommonLog _logger;
        private readonly IOperationScopeFactory _scopeFactory;

        public MergeClientsHandler(ISubRequestProcessor subRequestProcessor, IMsCrmSettings msCrmSettings, IClientRepository clientRepository, ICommonLog logger, IOperationScopeFactory scopeFactory)
        {
            _subRequestProcessor = subRequestProcessor;
            _msCrmSettings = msCrmSettings;
            _clientRepository = clientRepository;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(MergeClientsRequest request)
        {
            if (request.AppendedClientId == request.Client.Id)
            {
                throw new NotificationException(BLResources.MergeClientsSameIdError);
            }

            Tuple<Client, Client> clients;
            using (var operationScope = _scopeFactory.CreateSpecificFor<MergeIdentity, Client>())
            {
                clients = _clientRepository.MergeErmClients(request.Client.Id, request.AppendedClientId, request.Client);

                operationScope
                    .Updated<Client>(request.Client.Id, request.AppendedClientId)
                    .Complete();
            }

            var mainClient = clients.Item1;
            var appendedClient = clients.Item2;
            // Replication
            _subRequestProcessor.HandleSubRequest(new AssignClientRelatedEntitiesRequest
                                                      {
                                                          OwnerCode = request.Client.OwnerCode,
                                                          ClientId = request.AppendedClientId
                                                      }, Context, false);

            _subRequestProcessor.HandleSubRequest(new AssignClientRelatedEntitiesRequest
                                                      {
                                                          OwnerCode = request.Client.OwnerCode,
                                                          ClientId = request.Client.Id
                                                      }, Context, false);

            if (_msCrmSettings.EnableReplication)
            {
                _logger.InfoEx("Начало репликации действий в CRM");
                try
                {
                    var crmDataContext = _msCrmSettings.CreateDataContext();

                    var crmMainClient =
                        crmDataContext.GetEntities(EntityName.account).SingleOrDefault(
                            x => x.GetPropertyValue<Guid>("accountid") == mainClient.ReplicationCode);
                    var crmAppendedClient =
                        crmDataContext.GetEntities(EntityName.account).SingleOrDefault(
                            x => x.GetPropertyValue<Guid>("accountid") == appendedClient.ReplicationCode);
                    if (crmAppendedClient != null && crmMainClient != null)
                    {
                        _subRequestProcessor.HandleSubRequest(new UpdateRelatedCrmActivitiesRequest
                        {
                                                                      CrmFromObjectCode = crmAppendedClient.Id.Value,
                                                                      CrmToObjectCode = mainClient.ReplicationCode,
                            CrmObjectType = EntityName.account
                        },
                                                                      Context);
                    }
                }
                catch (WebException ex)
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
                }
            }

            return Response.Empty;
        }
    }
}
