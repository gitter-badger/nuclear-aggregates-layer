using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;

using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

namespace DoubleGis.Erm.BL.Operations.Concrete.Limits
{
    // TODO {all, 17.12.2014}: Идеально было бы отказаться от данного operation service, реализовав вместо него отдельные SRP cервисы Reject, Approve, Open
    // + к каждому из них aggregate service со специфичной сигнатурой, тогда и проверки на допустимость переходов статусов упростятся
    public sealed class SetLimitStatusOperationService : ISetLimitStatusOperationService
    {
        private readonly ISecurityServiceFunctionalAccess _securityService;
        private readonly IActionLogger _actionLogger;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IUserContext _userContext;
        private readonly IUpdateLimitAggregateService _updateLimitAggregateService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IDictionary<LimitStatus, LimitStatus[]> _allowedTransitions = new Dictionary<LimitStatus, LimitStatus[]>
            {
                { LimitStatus.Opened, new[] { LimitStatus.Rejected, LimitStatus.Approved } },
                { LimitStatus.Rejected, new[] { LimitStatus.Opened } },
                { LimitStatus.Approved, new[] { LimitStatus.Opened } }
            };

        public SetLimitStatusOperationService(
            IAccountReadModel accountReadModel,
            IActionLogger actionLogger,
            IUserContext userContext,
            ISecurityServiceFunctionalAccess securityService,
            IUpdateLimitAggregateService updateLimitAggregateService,
            IOperationScopeFactory operationScopeFactory)
        {
            _accountReadModel = accountReadModel;
            _userContext = userContext;
            _securityService = securityService;
            _updateLimitAggregateService = updateLimitAggregateService;
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
        }

        // TODO {all, 17.12.2014}: limitReplicationCodes лучше выпилить - пусть либо клиент выполняет resolve crmid->ermid, либо сontroller там же можно заблокировать групповые вызовы.
        public void SetStatus(long limitId, LimitStatus status, params Guid[] limitReplicationCodes)
        {
            // Проверка касается только функциональных разрешений, если не хватает сущностных,
            // тогда уровень доступа к данным создаст исключение, которое будет поймано ниже.
            if (!HasFunctionalAccess(status))
            {
                throw new OperationAccessDeniedException(BLResources.InsufficientRightsWarning);
            }

            try
            {
                using (var scope = _operationScopeFactory.CreateNonCoupled<SetLimitStatusIdentity>())
                {
                    var limit = GetLimit(limitId, limitReplicationCodes);
                    if (limit == null)
                    {
                        throw new EntityNotFoundException(typeof(Limit));
                    }

                    CheckTransition(limit.Status, status);

                    string name;
                    if (_accountReadModel.TryGetLimitLockingRelease(limit, out name))
                    {
                        throw new LimitIsBlockedByReleaseException(string.Format(BLResources.LimitEditBlockedByRelease, name));
                    }

                    if (!limit.IsActive)
                    {
                        throw new InactiveEntityModificationException();
                    }

                    var originalLimitObject = CompareObjectsHelper.CreateObjectDeepClone(limit);

                    limit.Status = status;
                    switch (status)
                    {
                        case LimitStatus.Rejected:
                        case LimitStatus.Approved:
                            limit.InspectorCode = _userContext.Identity.Code;
                            break;
                        case LimitStatus.Opened:
                            limit.Amount = _accountReadModel.CalculateLimitValueForAccountByPeriod(limit.AccountId, limit.StartPeriodDate, limit.EndPeriodDate);
                            break;
                    }

                    _updateLimitAggregateService.Update(limit, _accountReadModel.GetAccountOwnerCode(limit.AccountId));
                    _actionLogger.LogChanges(originalLimitObject, limit);
                    scope.Updated(limit)
                         .Complete();
                }
            }
            catch (SecurityAccessDeniedException e)
            {
                throw new OperationAccessDeniedException(BLResources.InsufficientRightsWarning, e);
            }
        }

        private static string GetStatusLocalizedName(LimitStatus currentStatus)
        {
            var statusName = Enum.GetName(typeof(LimitStatus), currentStatus);
            return string.IsNullOrEmpty(statusName) ? string.Empty : BLResources.ResourceManager.GetString(statusName);
        }

        private Limit GetLimit(long limitId, Guid[] limitReplicationCodes)
        {
            var replicationCodes = limitReplicationCodes;
            if (replicationCodes.Length > 1)
            {
                throw new NotificationException(BLResources.LimitStatusChangeIsNotSupportedAsTheGroupOperation);
            }

            return (limitId > 0)
                       ? _accountReadModel.GetLimitById(limitId)
                       : _accountReadModel.GetLimitByReplicationCode(replicationCodes.FirstOrDefault());
        }

        private void CheckTransition(LimitStatus statusFrom, LimitStatus statusTo)
        {
            LimitStatus[] allowedTransitions;
            if (!_allowedTransitions.TryGetValue(statusTo, out allowedTransitions))
            {
                throw new LimitWorkflowIsViolatedException(string.Format(BLResources.NotExistsTransitionForLimitStatusErrorMessage, GetStatusLocalizedName(statusTo)));
            }

            if (allowedTransitions.Contains(statusFrom))
            {
                return;
            }

            var message = string.Format(BLResources.SetLimitStatusWrongTransitionErrorMessage,
                                        GetStatusLocalizedName(statusTo),
                                        string.Join(",", allowedTransitions.Select(GetStatusLocalizedName)));

            throw new LimitWorkflowIsViolatedException(message);
        }

        private bool HasFunctionalAccess(LimitStatus newStatus)
        {
            switch (newStatus)
            {
                case LimitStatus.Opened:
                    // Проверка доступа на изменение будет произведена при вызове _limitCrudService.Modify(limit); поэтому для статуса Opened ничего не делаем
                    return true;
                case LimitStatus.Approved:
                case LimitStatus.Rejected:
                    return _securityService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitManagement, _userContext.Identity.Code);
                default:
                    throw new ArgumentOutOfRangeException("newStatus");
            }
        }
    }
}
