using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Qualify
{
    public class QualifyFirmService : IQualifyGenericEntityService<Firm>
    {
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICommonLog _logger;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ICreateClientByFirmOperationService _createClientByFirmAggregateService;
        private readonly IClientReadModel _clientReadModel;

        public QualifyFirmService(
            IUserContext userContext,
            IFirmRepository firmRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            ICommonLog logger,
            IOperationScopeFactory operationScopeFactory,
            ICreateClientByFirmOperationService createClientByFirmAggregateService,
            IClientReadModel clientReadModel)
        {
            _userContext = userContext;
            _firmRepository = firmRepository;
            _userIdentifierService = userIdentifierService;
            _logger = logger;
            _operationScopeFactory = operationScopeFactory;
            _createClientByFirmAggregateService = createClientByFirmAggregateService;
            _clientReadModel = clientReadModel;
        }

        public virtual QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId)
        {
            var currentUser = _userContext.Identity;
            var reserveUser = _userIdentifierService.GetReserveUserIdentity();

            Client client;

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<QualifyIdentity, Firm>())
            {
                var firm = _firmRepository.GetFirm(entityId);
                if (firm == null)
                {
                    throw new ArgumentException(BLResources.CouldNotFindFirm);
                }

                _firmRepository.Qualify(firm, currentUser.Code, reserveUser.Code, ownerCode, DateTime.UtcNow);

                if (relatedEntityId != null)
                {
                    var clientId = relatedEntityId.Value;
                    client = _clientReadModel.GetClient(clientId);
                    if (client == null)
                    {
                        throw new ArgumentException(string.Format(BLResources.CouldNotFindClient, clientId));
                    }

                    if (client.OwnerCode == reserveUser.Code)
                    {
                        throw new ArgumentException(BLResources.QualifyCouldnotPickReserveClient);
                    }

                    if (client.OwnerCode != ownerCode)
                    {
                        throw new ArgumentException(BLResources.QualifyCurrentUserIsNotOwnerForClient);
                    }

                    _firmRepository.SetFirmClient(firm, client.Id);
                    operationScope.Updated<Firm>(firm.Id);
                }
                else
                {
                    client = _createClientByFirmAggregateService.CreateByFirm(firm, ownerCode);
                    operationScope.Added<Client>(client.Id);
                }

                operationScope.Updated<Firm>(entityId);
                operationScope.Complete();
            }

            _logger.InfoFormat("Фирма с id={0} взята из резерва, с назначением пользователю {1}", entityId, ownerCode);

            return new QualifyResult
                       {
                           EntityId = entityId,
                           RelatedEntityId = client.Id
                       };
        }
    }
}