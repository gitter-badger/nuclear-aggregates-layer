using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Qualify
{
    public class QualifyClientService : IQualifyGenericEntityService<Client>
    {
        private readonly IUserContext _userContext;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IPublicService _publicService;
        private readonly ICommonLog _logger;

        public QualifyClientService(
            IUserContext userContext,
            IClientRepository clientRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            IOperationScopeFactory scopeFactory,
            IPublicService publicService, 
            ICommonLog logger)
        {
            _userContext = userContext;
            _clientRepository = clientRepository;
            _userIdentifierService = userIdentifierService;
            _scopeFactory = scopeFactory;
            _publicService = publicService;
            _logger = logger;
        }

        public virtual QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId)
        {
            var currentUser = _userContext.Identity;
            var reserveUser = _userIdentifierService.GetReserveUserIdentity();

            using (var operationScope = _scopeFactory.CreateSpecificFor<QualifyIdentity, Client>())
            {
                var qualifyAggregateRepository = _clientRepository as IQualifyAggregateRepository<Client>;
                qualifyAggregateRepository.Qualify(entityId, currentUser.Code, reserveUser.Code, ownerCode, DateTime.UtcNow);

                operationScope
                    .Updated<Client>(entityId)
                    .Complete();
            }

            _logger.InfoFormatEx("[ERM] Клиент с id={0} взят из резерва, с назначением пользователю с id={1}", entityId, ownerCode);
            _publicService.Handle(new AssignClientRelatedEntitiesRequest { ClientId = entityId, OwnerCode = ownerCode });
            _logger.InfoFormatEx("[CRM] Клиент с id={0} взят из резерва, с назначением пользователю c id={1}", entityId, ownerCode);

            return new QualifyResult();
        }
    }
}