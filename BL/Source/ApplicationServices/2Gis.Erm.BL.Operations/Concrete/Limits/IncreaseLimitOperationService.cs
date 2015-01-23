using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.Operations.Concrete.Limits
{
    public sealed class IncreaseLimitOperationService : IIncreaseLimitOperationService
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IActionLogger _actionLogger;
        private readonly IUpdateLimitAggregateService _updateLimitAggregateService;

        public IncreaseLimitOperationService(ISecurityServiceFunctionalAccess securityServiceFunctionalAccess,
                                             ISecurityServiceEntityAccess securityServiceEntityAccess,
                                             IUserContext userContext,
                                             IAccountReadModel accountReadModel,
                                             IOperationScopeFactory operationScopeFactory,
                                             IActionLogger actionLogger,
                                             IUpdateLimitAggregateService updateLimitAggregateService)
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
            _accountReadModel = accountReadModel;
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _updateLimitAggregateService = updateLimitAggregateService;
        }

        public void IncreaseLimit(long limitId, decimal amountToIncrease)
        {
            if (amountToIncrease <= 0)
            {
                throw new LimitIncreasingAmountException(Resources.Server.Properties.Resources.WrongAmountToIncreaseLimit);
            }

            var limit = _accountReadModel.GetLimitById(limitId);

            if (limit.Status != LimitStatus.Approved)
            {
                throw new LimitWorkflowIsViolatedException(Resources.Server.Properties.Resources.OnlyApprovedLimitCanBeIncreased);
            }

            var hasFunctionalPrivelege = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitManagement, _userContext.Identity.Code);
            var hasEntityAccess = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                               EntityType.Instance.Limit(),
                                                                               _userContext.Identity.Code,
                                                                               limitId,
                                                                               limit.OwnerCode,
                                                                               null);

            if (!hasFunctionalPrivelege || !hasEntityAccess)
            {
                throw new OperationAccessDeniedException(IncreaseLimitIdentity.Instance);
            }

            using (var scope = _operationScopeFactory.CreateNonCoupled<IncreaseLimitIdentity>())
            {
                var calculatedAmountToIncrease = _accountReadModel.CalculateLimitIncreasingValue(limitId);
                var originalLimitObject = CompareObjectsHelper.CreateObjectDeepClone(limit);
                if (calculatedAmountToIncrease != amountToIncrease)
                {
                    throw new LimitIncreasingAmountIsOutdatedException(Resources.Server.Properties.Resources.AmountToIncreaseLimitIsOutdated);
                }

                limit.Amount += amountToIncrease;

                _updateLimitAggregateService.Update(limit, _accountReadModel.GetAccountOwnerCode(limit.AccountId));
                _actionLogger.LogChanges(originalLimitObject, limit);

                scope.Updated(limit)
                     .Complete();
            }
        }
    }
}