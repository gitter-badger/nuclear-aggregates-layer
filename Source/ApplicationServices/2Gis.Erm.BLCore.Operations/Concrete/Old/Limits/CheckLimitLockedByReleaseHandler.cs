using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Limits
{
    public sealed class CheckLimitLockedByReleaseHandler : RequestHandler<CheckLimitLockedByReleaseRequest, EmptyResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public CheckLimitLockedByReleaseHandler(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        protected override EmptyResponse Handle(CheckLimitLockedByReleaseRequest request)
        {
            string name;
            if (_accountRepository.TryGetLimitLockingRelease(request.Entity, out name))
            {
                throw new NotificationException(string.Format(BLResources.LimitEditBlocedByRelease, name));
            }

            return Response.Empty;
        }
    }
}
