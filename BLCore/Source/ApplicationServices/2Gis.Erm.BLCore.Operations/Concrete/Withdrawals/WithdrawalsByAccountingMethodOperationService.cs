using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class WithdrawalsByAccountingMethodOperationService : IWithdrawalsByAccountingMethodOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IWithdrawalOperationService _withdrawalOperationService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ICheckOperationPeriodService _checkOperationPeriodService;
        private readonly IUserContext _userContext;
        private readonly IUseCaseTuner _useCaseTuner;

        public WithdrawalsByAccountingMethodOperationService(IAccountReadModel accountReadModel,
                                                             IWithdrawalOperationService withdrawalOperationService,
                                                             IOperationScopeFactory operationScopeFactory,
                                                             ISecurityServiceFunctionalAccess functionalAccessService,
                                                             ICheckOperationPeriodService checkOperationPeriodService,
                                                             IUserContext userContext,
                                                             IUseCaseTuner useCaseTuner)
        {
            _accountReadModel = accountReadModel;
            _withdrawalOperationService = withdrawalOperationService;
            _operationScopeFactory = operationScopeFactory;
            _functionalAccessService = functionalAccessService;
            _checkOperationPeriodService = checkOperationPeriodService;
            _userContext = userContext;
            _useCaseTuner = useCaseTuner;
        }

        public IDictionary<long, WithdrawalProcessingResult> Withdraw(TimePeriod period, AccountingMethod accountingMethod)
        {
            _useCaseTuner.AlterDuration<WithdrawalsByAccountingMethodOperationService>();

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException("User doesn't have sufficient privileges for managing withdrawal");
            }

            string report;
            if (!_checkOperationPeriodService.IsOperationPeriodValid(period, out report))
            {
                throw new InvalidPeriodException(report);
            }

            //using (var scope = _operationScopeFactory.CreateNonCoupled<WithdrawalsByAccountingMethodIdentity>())
            {
                var organizationUnits = _accountReadModel.GetOrganizationUnitsToProccessWithdrawals(period.Start, period.End, accountingMethod);
                var result = organizationUnits.ToDictionary(organizationUnit => organizationUnit,
                                                            organizationUnit => _withdrawalOperationService.Withdraw(organizationUnit, period, accountingMethod));

               // scope.Complete();

                return result;
            }
        }
    }
}
