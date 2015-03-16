using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.CheckForDebts
{
    public class CheckAccountForDebtsService : ICheckGenericEntityForDebtsService<Account>
    {
        private readonly IUserContext _userContext;
        private readonly IAccountDebtsChecker _accountDebtsChecker;

        public CheckAccountForDebtsService(IUserContext userContext, IAccountDebtsChecker accountDebtsChecker)
        {
            _userContext = userContext;
            _accountDebtsChecker = accountDebtsChecker;
        }

        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            try
            {
                _accountDebtsChecker.Check(false, _userContext.Identity.Code, () => new[] { entityId }, delegate { });
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