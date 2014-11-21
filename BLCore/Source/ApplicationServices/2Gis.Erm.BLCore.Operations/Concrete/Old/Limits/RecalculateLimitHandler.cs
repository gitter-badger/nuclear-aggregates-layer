using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Limits
{
    public sealed class RecalculateLimitHandler : RequestHandler<RecalculateLimitRequest, EmptyResponse>
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IActionLogger _actionLogger;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserContext _userContext;

        public RecalculateLimitHandler(
            IUserContext userContext, 
            IAccountRepository accountRepository, 
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess securityServiceEntityAccess,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess)
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _accountRepository = accountRepository;
            _actionLogger = actionLogger;
            _userContext = userContext;
        }

        protected override EmptyResponse Handle(RecalculateLimitRequest request)
        {
            var limit = _accountRepository.FindLimit(request.LimitId);
            AuthorizeUser(_userContext.Identity.Code, limit);

            var originalLimitObject = CompareObjectsHelper.CreateObjectDeepClone(limit);
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _accountRepository.RecalculateLimitValue(limit, limit.StartPeriodDate, limit.EndPeriodDate);
                _accountRepository.Update(limit);
                _actionLogger.LogChanges(originalLimitObject, limit);
                transaction.Complete();
            }
            return Response.Empty;
        }

        private void AuthorizeUser(long userId, Limit limit)
        {
            var hasFunctionalPrivelege = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitRecalculation, userId);
            var hasEntityAccess = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update, EntityName.Limit, userId, limit.Id, limit.OwnerCode, null);

            if (!hasFunctionalPrivelege || !hasEntityAccess)
            {
                throw new NotificationException(BLResources.UserIsNotAllowedToRecalculateLimit);
            }
        }
    }
}
