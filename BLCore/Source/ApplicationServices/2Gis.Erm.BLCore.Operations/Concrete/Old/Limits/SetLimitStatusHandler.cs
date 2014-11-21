using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Limits
{
    public sealed class SetLimitStatusHandler : RequestHandler<SetLimitStatusRequest, Response>
    {
        private readonly ISecurityServiceFunctionalAccess _securityService;
        private readonly IActionLogger _actionLogger;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IUserContext _userContext;

        private readonly IDictionary<LimitStatus, LimitStatus[]> _allowedTransitions = new Dictionary<LimitStatus, LimitStatus[]>
            {
                { LimitStatus.Opened, new[] { LimitStatus.Rejected, LimitStatus.Approved } },
                { LimitStatus.Rejected, new[] { LimitStatus.Opened } },
                { LimitStatus.Approved, new[] { LimitStatus.Opened } }
            };

        public SetLimitStatusHandler(
            IAccountReadModel accountReadModel,
            IAccountRepository accountRepository,
            IActionLogger actionLogger,
            IUserContext userContext,
            ISecurityServiceFunctionalAccess securityService)
        {
            _accountReadModel = accountReadModel;
            _userContext = userContext;
            _securityService = securityService;
            _accountRepository = accountRepository;
            _actionLogger = actionLogger;
        }

        protected override Response Handle(SetLimitStatusRequest request)
        {
            // Проверка касается только функциональных разрешений, если не хватает сущностных,
            // тогда уровень доступа к данным создаст исключение, которое будет поймано ниже.
            if (!HasFunctionalAccess(request.Status))
            {
                throw new NotificationException(BLResources.InsufficientRightsWarning);
            }

            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    var limit = GetLimit(request);
                    if (limit == null)
                    {
                        throw new NotificationException(BLResources.EntityNotFound);
                    }

                    CheckTransition((LimitStatus)limit.Status, request.Status);

                    string name;
                    if (_accountReadModel.TryGetLimitLockingRelease(limit, out name))
                    {
                        throw new NotificationException(string.Format(BLResources.LimitEditBlockedByRelease, name));
                    }

                    if (!limit.IsActive)
                    {
                        throw new NotificationException(BLResources.EntityIsInactive);
                    }

                    var originalLimitObject = CompareObjectsHelper.CreateObjectDeepClone(limit);

                    limit.Status = (short)request.Status;
                    switch (request.Status)
                    {
                        case LimitStatus.Rejected:
                        case LimitStatus.Approved:
                            limit.InspectorCode = _userContext.Identity.Code;
                            break;
                        case LimitStatus.Opened:
                            _accountRepository.RecalculateLimitValue(limit, limit.StartPeriodDate, limit.EndPeriodDate);
                            break;
                    }

                    _accountRepository.Update(limit);
                    _actionLogger.LogChanges(originalLimitObject, limit);
                    transaction.Complete();
                }
            }
            catch (SecurityAccessDeniedException e)
            {
                throw new NotificationException(BLResources.InsufficientRightsWarning, e);
            }

            return Response.Empty;
        }

        private static string GetStatusLocalizedName(LimitStatus currentStatus)
        {
            var statusName = Enum.GetName(typeof(LimitStatus), currentStatus);
            return string.IsNullOrEmpty(statusName) ? string.Empty : BLResources.ResourceManager.GetString(statusName);
        }

        private Limit GetLimit(SetLimitStatusRequest request)
        {
            var replicationCodes = request.LimitReplicationCodes;
            if (replicationCodes.Length > 1)
            {
                throw new NotificationException(BLResources.LimitStatusChangeIsNotSupportedAsTheGroupOperation);
            }

            return (request.LimitId > 0)
                       ? _accountRepository.GetLimitById(request.LimitId)
                       : _accountRepository.GetLimitByReplicationCode(replicationCodes.FirstOrDefault());
        }

        private void CheckTransition(LimitStatus statusFrom, LimitStatus statusTo)
        {
            LimitStatus[] allowedTransitions;
            if (!_allowedTransitions.TryGetValue(statusTo, out allowedTransitions))
            {
                throw new NotificationException(string.Format(BLResources.NotExistsTransitionForLimitStatusErrorMessage, GetStatusLocalizedName(statusTo)));
            }

            if (allowedTransitions.Contains(statusFrom))
            {
                return;
            }

            var message = string.Format(BLResources.SetLimitStatusWrongTransitionErrorMessage,
                                        GetStatusLocalizedName(statusTo),
                                        string.Join(",", allowedTransitions.Select(GetStatusLocalizedName)));

            throw new NotificationException(message);
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
