using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

namespace DoubleGis.Erm.BL.Operations.Concrete.Limits
{
    public sealed class RecalculateLimitOperationService : IRecalculateLimitOperationService
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IActionLogger _actionLogger;
        private readonly IUserContext _userContext;
        private readonly IUpdateLimitAggregateService _updateLimitAggregateService;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public RecalculateLimitOperationService(
            IUserContext userContext,
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess securityServiceEntityAccess,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess,
            IUpdateLimitAggregateService updateLimitAggregateService,
            IAccountReadModel accountReadModel,
            IOperationScopeFactory operationScopeFactory)
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _updateLimitAggregateService = updateLimitAggregateService;
            _accountReadModel = accountReadModel;
            _operationScopeFactory = operationScopeFactory;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _actionLogger = actionLogger;
            _userContext = userContext;
        }

        public void Recalculate(long limitId)
        {
            if (!_accountReadModel.IsLimitRecalculationAvailable(limitId))
            {
                throw new OperationAccessDeniedException(RecalculateLimitIdentity.Instance);
            }

            var limit = _accountReadModel.GetLimitById(limitId);
            AuthorizeUser(_userContext.Identity.Code, limit);

            var originalLimitObject = CompareObjectsHelper.CreateObjectDeepClone(limit);
            using (var scope = _operationScopeFactory.CreateNonCoupled<RecalculateLimitIdentity>())
            {
                limit.Amount = _accountReadModel.CalculateLimitValueForAccountByPeriod(limit.AccountId, limit.StartPeriodDate, limit.EndPeriodDate);
                _updateLimitAggregateService.Update(limit, _accountReadModel.GetAccountOwnerCode(limit.AccountId));
                _actionLogger.LogChanges(originalLimitObject, limit);

                scope.Updated(limit)
                     .Complete();
            }
        }

        private void AuthorizeUser(long userId, Limit limit)
        {
            var hasFunctionalPrivelege = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitRecalculation, userId);
            var hasEntityAccess = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update, EntityName.Limit, userId, limit.Id, limit.OwnerCode, null);

            if (!hasFunctionalPrivelege || !hasEntityAccess)
            {
                throw new OperationAccessDeniedException(BLResources.UserIsNotAllowedToRecalculateLimit);
            }
        }
    }
}
