using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.Operations.Concrete.Limits
{
    public sealed class CalculateLimitIncreasingOperationService : ICalculateLimitIncreasingOperationService
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CalculateLimitIncreasingOperationService(ISecurityServiceFunctionalAccess securityServiceFunctionalAccess,
                                                        ISecurityServiceEntityAccess securityServiceEntityAccess,
                                                        IUserContext userContext,
                                                        IAccountReadModel accountReadModel,
                                                        IOperationScopeFactory operationScopeFactory)
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
            _accountReadModel = accountReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public bool IsIncreasingRequired(long limitId, out decimal amountToIncrease)
        {
            var limitOwnerCode = _accountReadModel.GetLimitOwnerCode(limitId);

            var hasFunctionalPrivelege = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitManagement, _userContext.Identity.Code);
            var hasEntityAccess = _securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                               EntityType.Instance.Limit(),
                                                                               _userContext.Identity.Code,
                                                                               limitId,
                                                                               limitOwnerCode,
                                                                               null);

            if (!hasFunctionalPrivelege || !hasEntityAccess)
            {
                throw new OperationAccessDeniedException(CalculateLimitIncreasingIdentity.Instance);
            }

            using (var scope = _operationScopeFactory.CreateNonCoupled<CalculateLimitIncreasingIdentity>())
            {
                amountToIncrease = _accountReadModel.CalculateLimitIncreasingValue(limitId);
                scope.Complete();

                return amountToIncrease > 0;
            }
        }
    }
}