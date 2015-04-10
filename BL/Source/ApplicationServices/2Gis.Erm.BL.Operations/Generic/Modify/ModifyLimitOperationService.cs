using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.Operations.Generic.Modify
{
    public class ModifyLimitOperationService : IModifyBusinessModelEntityService<Limit>
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly IUserContext _userContext;
        private readonly IActionLogger _actionLogger;
        private readonly ICreateLimitAggregateService _createLimitAggregateService;
        private readonly IUpdateLimitAggregateService _updateLimitAggregateService;
        private readonly IBusinessModelEntityObtainer<Limit> _limitObtainer;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ModifyLimitOperationService(IAccountReadModel accountReadModel,
                                           ISecurityServiceFunctionalAccess securityServiceFunctionalAccess,
                                           IUserContext userContext,
                                           IActionLogger actionLogger,
                                           ICreateLimitAggregateService createLimitAggregateService,
                                           IUpdateLimitAggregateService updateLimitAggregateService,
                                           IBusinessModelEntityObtainer<Limit> limitObtainer,
                                           IOperationScopeFactory operationScopeFactory)
        {
            _accountReadModel = accountReadModel;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _userContext = userContext;
            _actionLogger = actionLogger;
            _createLimitAggregateService = createLimitAggregateService;
            _updateLimitAggregateService = updateLimitAggregateService;
            _limitObtainer = limitObtainer;
            _operationScopeFactory = operationScopeFactory;
        }

        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            var limit = _limitObtainer.ObtainBusinessModelEntity(domainEntityDto);
            if (limit.Amount <= 0)
            {
                throw new LimitAmountException(BLResources.WrongLimitAmount);
            }

            // Лимит предоставляется на один месяц, поэтому смысл имеет только дата начала.
            limit.EndPeriodDate = limit.StartPeriodDate.GetEndPeriodOfThisMonth();

            using (var scope = _operationScopeFactory.CreateOrUpdateOperationFor(limit))
            {
                // Если лимит менять нельзя, кинет исключение
                string name;
                if (_accountReadModel.TryGetLimitLockingRelease(limit, out name))
                {
                    throw new LimitIsBlockedByReleaseException(string.Format(BLResources.LimitEditBlockedByRelease, name));
                }

                if (!limit.IsActive)
                {
                    throw new InactiveEntityModificationException();
                }

                // Поскольку у лимита могли сменить период, следует убедиться, что он не конфликтует с уже существующими.
                var isConflictingLimitExists = _accountReadModel.IsThereLimitDuplicate(limit.Id, limit.AccountId, limit.StartPeriodDate, limit.EndPeriodDate);
                if (isConflictingLimitExists)
                {
                    throw new EntityIsNotUniqueException(typeof(Limit), BLResources.CreatingLimitImpossibleBecauseAnotherLimitExists);
                }

                var originalLimitObject = _accountReadModel.GetLimitById(limit.Id);
                if (originalLimitObject != null && PeriodChanged(originalLimitObject, limit) && !FunctionalPrivelegeGranted())
                {
                    // Смена периода для уже существующего лимита возможна только при налчии спец. разрешения.
                    throw new OperationAccessDeniedException(Resources.Server.Properties.Resources.AccessToChangePeriodOfLimitIsDenied);
                }

                if (limit.IsNew())
                {
                    _createLimitAggregateService.Create(limit, _accountReadModel.GetAccountOwnerCode(limit.AccountId));
                    scope.Added(limit);
                }
                else
                {
                    _updateLimitAggregateService.Update(limit, _accountReadModel.GetAccountOwnerCode(limit.AccountId));
                    scope.Updated(limit);
                }

                if (originalLimitObject != null)
                {
                    _actionLogger.LogChanges(originalLimitObject, limit);
                }

                scope.Complete();
            }

            return limit.Id;
        }

        private static bool PeriodChanged(Limit originalLimitObject, Limit modifiedLimitObject)
        {
            return originalLimitObject.StartPeriodDate != modifiedLimitObject.StartPeriodDate ||
                   originalLimitObject.EndPeriodDate != modifiedLimitObject.EndPeriodDate;
        }

        private bool FunctionalPrivelegeGranted()
        {
            return _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LimitPeriodChanging, _userContext.Identity.Code);
        }
    }
}