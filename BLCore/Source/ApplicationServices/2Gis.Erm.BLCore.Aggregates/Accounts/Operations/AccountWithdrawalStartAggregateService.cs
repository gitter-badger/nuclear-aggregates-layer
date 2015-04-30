using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountWithdrawalStartAggregateService : IAccountWithdrawalStartAggregateService
    {
        private readonly IRepository<WithdrawalInfo> _withdrawalRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public AccountWithdrawalStartAggregateService(
            IRepository<WithdrawalInfo> withdrawalRepository,
            IUserContext userContext,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _withdrawalRepository = withdrawalRepository;
            _userContext = userContext;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public WithdrawalInfo Start(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            if (accountingMethod == AccountingMethod.Undefined)
            {
                throw new AccountingMethodViolationException(accountingMethod);
            }

            WithdrawalInfo newWithdrawal;
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, WithdrawalInfo>())
            {
                newWithdrawal = new WithdrawalInfo
                    {
                        StartDate = DateTime.UtcNow,
                        PeriodStartDate = period.Start,
                        PeriodEndDate = period.End,
                        OrganizationUnitId = organizationUnitId,
                        Status = WithdrawalStatus.Withdrawing,
                        AccountingMethod = accountingMethod,
                        OwnerCode = _userContext.Identity.Code,
                        IsActive = true
                    };

                _identityProvider.SetFor(newWithdrawal);
                _withdrawalRepository.Add(newWithdrawal);
                scope.Added<WithdrawalInfo>(newWithdrawal.Id);

                _withdrawalRepository.Save();
                scope.Complete();
            }

            return newWithdrawal;
        }
    }
}