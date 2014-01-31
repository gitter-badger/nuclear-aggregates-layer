using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditLimitHandler : RequestHandler<EditRequest<Limit>, EmptyResponse>
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly IUserContext _userContext;
        private readonly IActionLogger _actionLogger;
        private readonly IAccountRepository _accountRepository;

        public EditLimitHandler(
            IAccountReadModel accountReadModel,
            IAccountRepository accountRepository,
            IActionLogger actionLogger,
            IUserContext userContext,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess)
        {
            _accountReadModel = accountReadModel;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _userContext = userContext;
            _actionLogger = actionLogger;
            _accountRepository = accountRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Limit> request)
        {
            var limit = request.Entity;
            if (limit.Amount <= 0)
            {
                throw new NotificationException(BLResources.WrongLimitAmount);
            }

            // ����� ��������������� �� ���� �����, ������� ����� ����� ������ ���� ������.
            limit.EndPeriodDate = limit.StartPeriodDate.GetEndPeriodOfThisMonth();

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                // ���� ����� ������ ������, ����� ����������
                string name;
                if (_accountReadModel.TryGetLimitLockingRelease(limit, out name))
                {
                    throw new NotificationException(string.Format(BLResources.LimitEditBlockedByRelease, name));
                }

                if (!limit.IsActive)
                {
                    throw new NotificationException(BLResources.EntityIsInactive);
                }

                // ��������� � ������ ����� ������� ������, ������� ���������, ��� �� �� ����������� � ��� �������������.
                var isConflictingLimitExists = _accountRepository.IsLimitExists(limit.AccountId, limit.StartPeriodDate, limit.EndPeriodDate, limit.Id);
                if (isConflictingLimitExists)
                {
                    throw new NotificationException(BLResources.CreatingLimitImpossibleBecauseAnotherLimitExists);
                }

                var originalLimitObject = _accountRepository.GetLimitById(limit.Id);
                if (originalLimitObject != null && PeriodChanged(originalLimitObject, limit) && !FunctionalPrivelegeGranted())
                {
                    // ����� ������� ��� ��� ������������� ������ �������� ������ ��� ������ ����. ����������.
                    throw new NotificationException("������������ ���� ��� ����� ������� ������");
                }

                if (limit.IsNew())
                {
                    _accountRepository.Create(limit);
                }
                else
                {
                    _accountRepository.Update(limit);
                }

                if (originalLimitObject != null)
                {
                    _actionLogger.LogChanges(originalLimitObject, limit);
                }

                transaction.Complete();
            }

            return Response.Empty;
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