using System;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Limits
{
    public sealed class CreateLimitHandler : RequestHandler<CreateLimitRequest, CreateLimitResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateLimitHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        protected override CreateLimitResponse Handle(CreateLimitRequest request)
        {
            var nextMonthDate = DateTime.UtcNow.Date.AddMonths(1);
            var periodStart = nextMonthDate.GetFirstDateOfMonth();
            var periodEnd = nextMonthDate.GetEndPeriodOfThisMonth();

            var limit = _accountRepository.InitializeLimitForAccount(request.AccountId, periodStart, periodEnd);
            return new CreateLimitResponse
                       {
                           LegalPersonId = limit.LegalPersonId,
                           LegalPersonName = limit.LegalPersonName,
                           BranchOfficeId = limit.BranchOfficeId,
                           BranchOfficeName = limit.BranchOfficeName,
                           LegalPersonOwnerId = limit.LegalPersonOwnerId,
                           Amount = limit.Amount,
                           Status = LimitStatus.Opened,
                           StartPeriodDate = periodStart,
                           EndPeriodDate = periodEnd
                       };
        }
    }
}