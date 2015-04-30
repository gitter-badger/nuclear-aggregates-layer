using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public sealed class AssignAccountOperationService : IAssignGenericEntityService<Account>
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IAssignAccountAggregateService _assignAccountAggregateService;
        private readonly IOwnerValidator _ownerValidator;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserContext _userContext;
        private readonly IAccountDebtsChecker _accountDebtsChecker;
        private readonly IActionLogger _actionLogger;

        public AssignAccountOperationService(IAccountReadModel accountReadModel,
                                             IAssignAccountAggregateService assignAccountAggregateService,
                                             ISecurityServiceEntityAccess entityAccessService,
                                             ISecurityServiceFunctionalAccess functionalAccessService,
                                             IOperationScopeFactory scopeFactory,
                                             IUserContext userContext,
                                             IOwnerValidator ownerValidator,
                                             IAccountDebtsChecker accountDebtsChecker,
                                             IActionLogger actionLogger)
        {
            _accountReadModel = accountReadModel;
            _assignAccountAggregateService = assignAccountAggregateService;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _scopeFactory = scopeFactory;
            _userContext = userContext;
            _ownerValidator = ownerValidator;
            _accountDebtsChecker = accountDebtsChecker;
            _actionLogger = actionLogger;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Account>())
            {
                var accountToAssign = _accountReadModel.GetInfoForAssignAccount(entityId);
                var account = accountToAssign.Account;

                _ownerValidator.CheckIsNotReserve(account);

                string message;
                if (_accountDebtsChecker.HasDebts(bypassValidation, _userContext.Identity.Code, () => new[] { entityId }, out message))
                {
                    if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code))
                    {
                        throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, message));
                    }

                    return new AssignResult
                               {
                                   EntityId = entityId,
                                   OwnerCode = ownerCode,
                                   CanProceed = true,
                                   Message = message
                               };
                }

                var ownerCanBeChanged = _entityAccessService.HasEntityAccess(EntityAccessTypes.Assign,
                                                                             EntityName.Account,
                                                                             _userContext.Identity.Code,
                                                                             entityId,
                                                                             ownerCode,
                                                                             account.OwnerCode);
                if (!ownerCanBeChanged)
                {
                    throw new SecurityException(BLResources.AssignAccountAccessDenied);
                }


                var changes = _assignAccountAggregateService.Assign(accountToAssign, ownerCode);
                _actionLogger.LogChanges(changes);

                operationScope.Complete();
            }

            return null;
        }
    }
}
