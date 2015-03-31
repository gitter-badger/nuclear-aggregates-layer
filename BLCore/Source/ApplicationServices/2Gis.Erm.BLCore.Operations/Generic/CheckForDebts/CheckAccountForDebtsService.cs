using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.CheckForDebts
{
    public class CheckAccountForDebtsService : ICheckGenericEntityForDebtsService<Account>
    {
        private readonly IUserContext _userContext;
        private readonly IAccountRepository _accountRepository;

        public CheckAccountForDebtsService(IUserContext userContext, IAccountRepository accountRepository)
        {
            _userContext = userContext;
            _accountRepository = accountRepository;
        }

        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            try
            {
                var checkAggregateForDebtsRepository = _accountRepository as ICheckAggregateForDebtsRepository<Account>;
                checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, false);
                return new CheckForDebtsResult();
            }
            catch (ProcessAccountsWithDebtsException exception)
            {
                return new CheckForDebtsResult
                {
                    DebtsExist = true,
                    Message = exception.Message
                };
            }
        }
    }
}