using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
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
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly IUserContext _userContext;
        private readonly IActionLogger _actionLogger;
        private readonly IAccountRepository _accountRepository;

        public EditLimitHandler(ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess, 
            IUserContext userContext,
            IActionLogger actionLogger,
            IAccountRepository accountRepository)
        {
            _subRequestProcessor = subRequestProcessor;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _userContext = userContext;
            _actionLogger = actionLogger;
            _accountRepository = accountRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Limit> request)
        {
            var modifiedLimitObject = request.Entity;

            if (modifiedLimitObject.Amount <= 0)
            {
                throw new NotificationException(BLResources.WrongLimitAmount);
            }

            // Ћимит предоставл€етс€ на один мес€ц, пожтому смысл имеет только дата начала.
            modifiedLimitObject.EndPeriodDate = modifiedLimitObject.StartPeriodDate.GetEndPeriodOfThisMonth();

            // ≈сли лимит мен€ть нельз€, кинет исключение
            _subRequestProcessor.HandleSubRequest(new CheckLimitLockedByReleaseRequest { Entity = modifiedLimitObject }, Context);

            // ѕоскольку у лимита могли сменить период, следует убедитьс€, что он не конфликтует с уже существующими.
            var isLimitExists = _accountRepository.IsLimitExists(modifiedLimitObject.AccountId, modifiedLimitObject.StartPeriodDate, modifiedLimitObject.EndPeriodDate, modifiedLimitObject.Id);
            if (isLimitExists)
            {
                throw new NotificationException(BLResources.CreatingLimitImpossibleBecauseAnotherLimitExists);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var originalLimitObject = _accountRepository.GetLimitById(modifiedLimitObject.Id);
                
                // —мена периода дл€ уже существующего лимита возможна только при налчии спец. разрешени€.
                if (originalLimitObject != null && (PeriodChanged(originalLimitObject, modifiedLimitObject) && !FunctionalPrivelegeGranted()))
                {
                    throw new NotificationException("Ќедостаточно прав дл€ смены периода лимита");
                }

                if (modifiedLimitObject.IsNew())
                {
                    _accountRepository.Create(modifiedLimitObject);
                }
                else
                {
                    _accountRepository.Update(modifiedLimitObject);
                }

                if (originalLimitObject != null)
                {
                    _actionLogger.LogChanges(originalLimitObject, modifiedLimitObject);
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