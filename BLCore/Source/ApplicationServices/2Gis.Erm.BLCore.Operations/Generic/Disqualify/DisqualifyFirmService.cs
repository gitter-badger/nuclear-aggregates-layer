using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Disqualify
{
    [Obsolete("На текущий момент в UI нет кнопки вызова возврата фирмы в резерв. Необходимо уточнить и удалить в случае ненадобности")]
    public class DisqualifyFirmService : IDisqualifyGenericEntityService<Firm>
    {
        private readonly IUserContext _userContext;
        private readonly IFirmRepository _firmRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICommonLog _logger;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IActivityReadService _activityReadService;

        public DisqualifyFirmService(
            IUserContext userContext,
            IFirmRepository firmRepository,
            ISecurityServiceUserIdentifier userIdentifierService, 
            ICommonLog logger,
            IOperationScopeFactory operationScopeFactory,
            IActivityReadService activityReadService)
        {
            _userContext = userContext;
            _firmRepository = firmRepository;
            _userIdentifierService = userIdentifierService;
            _logger = logger;
            _operationScopeFactory = operationScopeFactory;
            _activityReadService = activityReadService;
        }

        // Метод должен быть виртуальным для работы ActionsHistory
        public virtual DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            // Проверяем открытые связанные объекты:
            // Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данной Фирмой, 
            // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данной Фирмой".
            var hasRelatedOpenedActivities = _activityReadService.CheckIfOpenActivityExistsRegarding(EntityName.Firm, entityId);
            if (hasRelatedOpenedActivities)
            {
                throw new NotificationException(BLResources.NeedToCloseAllActivities);
            }

            var reserveUser = _userIdentifierService.GetReserveUserIdentity();
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DisqualifyIdentity, Firm>())
            {
                var disqualifyAggregateRepository = _firmRepository as IDisqualifyAggregateRepository<Firm>;
                disqualifyAggregateRepository.Disqualify(entityId, _userContext.Identity.Code, reserveUser.Code, false, DateTime.UtcNow);

                operationScope.Updated<Firm>(entityId);
                operationScope.Complete();
            }

            _logger.InfoFormat("Фирма с id={0} возвращена в резерв", entityId);

            return null;
        }
    }
}