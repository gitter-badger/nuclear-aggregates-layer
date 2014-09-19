﻿using System;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Disqualify
{
    public class DisqualifyClientService : IDisqualifyGenericEntityService<Client>
    {
        private readonly IUserContext _userContext;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IPublicService _publicService;
        private readonly ICommonLog _logger;
        private readonly IClientReadModel _clientReadModel;
        private readonly IActivityReadService _activityReadService;

        public DisqualifyClientService(IUserContext userContext,
                                       IClientRepository clientRepository,
                                       ISecurityServiceUserIdentifier userIdentifierService,
                                       ISecurityServiceFunctionalAccess functionalAccessService,
                                       IPublicService publicService,
                                       ICommonLog logger,
                                       ISecurityServiceEntityAccess securityServiceEntityAccess,
                                       IClientReadModel clientReadModel,
                                       IActivityReadService activityReadService)
        {
            _userContext = userContext;
            _clientRepository = clientRepository;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _logger = logger;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _clientReadModel = clientReadModel;
            _activityReadService = activityReadService;
        }

        public virtual DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            var client = _clientReadModel.GetClient(entityId);
            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                              EntityName.Client,
                                                              _userContext.Identity.Code,
                                                              client.Id,
                                                              client.OwnerCode,
                                                              null))
            {
                throw new NotificationException(string.Format(BLResources.ClientDisqualificationIsDeniedForTheUser, client.Name));
            }

            try
            {
                // Проверяем открытые связанные объекты:
                // Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данным Клиентом и его фирмами, 
                // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данным Клиентом и его фирмами".
                var hasRelatedOpenedActivities = _activityReadService.CheckIfRelatedActivitiesExists(EntityName.Client, entityId);
                if (hasRelatedOpenedActivities)
                {
                    throw new NotificationException(BLResources.NeedToCloseAllActivities);
                }

                var reserveUser = _userIdentifierService.GetReserveUserIdentity();
                _publicService.Handle(new AssignClientRelatedEntitiesRequest { OwnerCode = reserveUser.Code, ClientId = entityId });

                // изменения всех сущностей ERM выполняем в отдельной транзакции, чтобы все изменения сущностей среплицировались в MSCRM и транзакция была закрыта
                var disqualifyAggregateRepository = _clientRepository as IDisqualifyAggregateRepository<Client>;
                disqualifyAggregateRepository.Disqualify(entityId, _userContext.Identity.Code, reserveUser.Code, bypassValidation, DateTime.UtcNow);


                _logger.InfoFormatEx("Клиент с id={0} возвращен в резерв", entityId);
                return null;
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, ex.Message), ex);
                }

                return new DisqualifyResult
                    {
                        EntityId = entityId,
                        CanProceed = true,
                        Message = ex.Message
                    };
            }
        }
    }
}