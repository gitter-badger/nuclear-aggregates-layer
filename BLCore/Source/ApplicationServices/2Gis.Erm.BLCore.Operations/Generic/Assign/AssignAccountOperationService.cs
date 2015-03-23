using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignAccountOperationService : IAssignGenericEntityService<Account>
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IAssignAccountAggregateService _assignAccountAggregateService;
        private readonly IOwnerValidator _ownerValidator;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserContext _userContext;
        private readonly ITracer _tracer;
        private readonly IAccountDebtsChecker _accountDebtsChecker;

        public AssignAccountOperationService(IAccountReadModel accountReadModel,
                                    IAssignAccountAggregateService assignAccountAggregateService,
            ISecurityServiceEntityAccess entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOperationScopeFactory scopeFactory,
            IUserContext userContext,
                                    ITracer tracer,
                                    IOwnerValidator ownerValidator,
                                    IAccountDebtsChecker accountDebtsChecker)
        {
            _accountReadModel = accountReadModel;
            _assignAccountAggregateService = assignAccountAggregateService;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _scopeFactory = scopeFactory;
            _userContext = userContext;
            _tracer = tracer;
            _ownerValidator = ownerValidator;
            _accountDebtsChecker = accountDebtsChecker;
        }

        // COMMENT {all, 16.03.2015}: Should be virtual for interception
        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            try
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Account>())
                {
                    _ownerValidator.CheckIsNotReserve<Account>(entityId);

                    _accountDebtsChecker.Check(bypassValidation, _userContext.Identity.Code, () => new[] { entityId }, delegate { });

                    var accountToAssign = _accountReadModel.GetInfoForAssignAccount(entityId);
                    var accountOwnerCode = accountToAssign.Account.OwnerCode;
                    if (!_userContext.Identity.SkipEntityAccessCheck)
                    {
                        var ownerCanBeChanged = _entityAccessService.HasEntityAccess(EntityAccessTypes.Assign,
                                                                                     EntityName.Account,
                                                                                     _userContext.Identity.Code,
                                                                                     entityId,
                                                                                     ownerCode,
                                                                                     accountOwnerCode);
                        if (!ownerCanBeChanged)
                        {
                            throw new SecurityException(BLResources.AssignAccountAccessDenied);
                        }
                    }

                    _assignAccountAggregateService.Assign(accountToAssign, ownerCode);

                    operationScope.Complete();
                }
                
                _tracer.InfoFormat("Куратором ЛС с id={0} назначен пользователь {1}", entityId, ownerCode);
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, ex.Message), ex);
                }

                return new AssignResult
                {
                    EntityId = entityId,
                    OwnerCode = ownerCode,
                    CanProceed = true,
                    Message = ex.Message
                };
            }

            return null;
        }
    }
}
