using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignAccountDetailService : IAssignGenericEntityService<AccountDetail>
    {
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;
        private readonly IAccountRepository _accountRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly ICommonLog _logger;

        public AssignAccountDetailService(
            IPublicService publicService, 
            IFinder finder, 
            IAccountRepository accountRepository,
            IOperationScopeFactory scopeFactory,
            IUserContext userContext,
            ISecurityServiceEntityAccess entityAccessService, 
            ICommonLog logger)
        {
            _publicService = publicService;
            _finder = finder;
            _accountRepository = accountRepository;
            _scopeFactory = scopeFactory;
            _entityAccessService = entityAccessService;
            _logger = logger;
            _userContext = userContext;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, AccountDetail>())
            {
                _publicService.Handle(new ValidateOwnerIsNotReserveRequest<AccountDetail> { Id = entityId });

                var accountDetailInfo = _finder.Find(Specs.Find.ById<AccountDetail>(entityId))
                    .Select(x => new
                        {
                            x.AccountId,
                            x.OwnerCode
                        })
                    .Single();

                _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Account> { Id = accountDetailInfo.AccountId });

                var ownerCanBeChanged = _entityAccessService.HasEntityAccess(EntityAccessTypes.Assign,
                                                                             EntityName.AccountDetail,
                                                                             _userContext.Identity.Code,
                                                                             entityId,
                                                                             ownerCode,
                                                                             accountDetailInfo.OwnerCode);
                if (!ownerCanBeChanged)
                {
                    throw new SecurityException(BLResources.AssignAccountDetailAccessDenied);
                }

                var assignAccountDetailRepository = _accountRepository as IAssignAggregateRepository<AccountDetail>;
                assignAccountDetailRepository.Assign(entityId, ownerCode);

                operationScope
                    .Updated<AccountDetail>(entityId)
                    .Complete();
            }

            _logger.InfoFormat("Куратором операции по ЛС с id={0} назначен пользователь {1}", entityId, ownerCode);

            return null;
        }
    }
}
