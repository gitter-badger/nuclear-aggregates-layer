using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.CheckForDebts
{
    public class CheckAccountForDebtsOperationService : ICheckGenericEntityForDebtsService<Account>
    {
        private readonly IUserContext _userContext;
        private readonly IAccountDebtsChecker _accountDebtsChecker;
        private readonly IOperationScopeFactory _scopeFactory;

        public CheckAccountForDebtsOperationService(IUserContext userContext, IAccountDebtsChecker accountDebtsChecker, IOperationScopeFactory scopeFactory)
        {
            _userContext = userContext;
            _accountDebtsChecker = accountDebtsChecker;
            _scopeFactory = scopeFactory;
        }

        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CheckForDebtsIdentity, Account>())
            {
                string message;
                var hasDebts = _accountDebtsChecker.HasDebts(false, _userContext.Identity.Code, () => new[] { entityId }, out message);
                scope.Complete();

                return new CheckForDebtsResult
                           {
                               DebtsExist = hasDebts,
                               Message = message
                           };
            }
        }
    }
}