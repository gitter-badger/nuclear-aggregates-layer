using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignAccountDetailOperationService : IAssignGenericEntityService<AccountDetail>
    {
        private readonly IOwnerValidator _ownerValidator;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly ITracer _tracer;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IAssignAccountDetailAggregateService _assignAccountDetailAggregateService;

        public AssignAccountDetailOperationService(IOwnerValidator ownerValidator,
            IOperationScopeFactory scopeFactory,
            ISecurityServiceEntityAccess entityAccessService, 
                                          IUserContext userContext,
                                          ITracer tracer,
                                          IAccountReadModel accountReadModel,
                                          IAssignAccountDetailAggregateService assignAccountDetailAggregateService)
        {
            _ownerValidator = ownerValidator;
            _scopeFactory = scopeFactory;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _tracer = tracer;
            _accountReadModel = accountReadModel;
            _assignAccountDetailAggregateService = assignAccountDetailAggregateService;
        }

        // COMMENT {all, 16.03.2015}: Should be virtual for interception
        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, AccountDetail>())
            {
                _ownerValidator.CheckIsNotReserve<AccountDetail>(entityId);

                var accountDetail = _accountReadModel.GetAccountDetail(entityId);
                var ownerCanBeChanged = _entityAccessService.HasEntityAccess(EntityAccessTypes.Assign,
                                                                             EntityName.AccountDetail,
                                                                             _userContext.Identity.Code,
                                                                             entityId,
                                                                             ownerCode,
                                                                             accountDetail.OwnerCode);
                if (!ownerCanBeChanged)
                {
                    throw new SecurityException(BLResources.AssignAccountDetailAccessDenied);
                }

                _assignAccountDetailAggregateService.Assign(accountDetail, ownerCode);

                operationScope.Complete();
            }

            _tracer.InfoFormat("Куратором операции по ЛС с id={0} назначен пользователь {1}", entityId, ownerCode);

            return null;
        }
    }
}
