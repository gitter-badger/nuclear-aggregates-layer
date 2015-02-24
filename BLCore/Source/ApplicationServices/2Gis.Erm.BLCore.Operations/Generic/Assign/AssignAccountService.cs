using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignAccountService : IAssignGenericEntityService<Account>
    {
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserContext _userContext;
        private readonly ICommonLog _logger;

        public AssignAccountService(
            IPublicService publicService,
            IFinder finder,
            IAccountRepository accountRepository,
            ISecurityServiceEntityAccess entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOperationScopeFactory scopeFactory,
            IUserContext userContext, 
            ICommonLog logger)
        {
            _publicService = publicService;
            _finder = finder;
            _accountRepository = accountRepository;
            _entityAccessService = entityAccessService;
            _functionalAccessService = functionalAccessService;
            _scopeFactory = scopeFactory;
            _userContext = userContext;
            _logger = logger;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            try
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Account>())
                {
                    _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Account> { Id = entityId });

                    var checkAggregateForDebtsRepository = _accountRepository as ICheckAggregateForDebtsRepository<Account>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, bypassValidation);

                    var accountOwnerCode = _finder.Find(Specs.Find.ById<Account>(entityId)).Select(x => x.OwnerCode).Single();
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

                    var assignAccountRepository = _accountRepository as IAssignAggregateRepository<Account>;
                    assignAccountRepository.Assign(entityId, ownerCode);

                    operationScope
                        .Updated<Account>(entityId)
                        .Complete();
                }
                
                _logger.InfoFormat("Куратором ЛС с id={0} назначен пользователь {1}", entityId, ownerCode);
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
