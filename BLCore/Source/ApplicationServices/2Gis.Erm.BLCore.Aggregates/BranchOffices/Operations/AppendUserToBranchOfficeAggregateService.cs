using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public sealed class AppendUserToBranchOfficeAggregateService : IAppendUserToBranchOfficeAggregateService
    {
        private readonly IRepository<UserBranchOffice> _userBranchOfficeRelationsRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public AppendUserToBranchOfficeAggregateService(
            IRepository<UserBranchOffice> userBranchOfficeRelationsRepository, 
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _userBranchOfficeRelationsRepository = userBranchOfficeRelationsRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void AppendUser(long userId, long branchOfficeId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, User, BranchOffice>())
            {
                var userBranchOffice = new UserBranchOffice { UserId = userId, BranchOfficeId = branchOfficeId };
                _identityProvider.SetFor(userBranchOffice);
                _userBranchOfficeRelationsRepository.Add(userBranchOffice);
                _userBranchOfficeRelationsRepository.Save();

                scope.Added(userBranchOffice)
                     .Updated<User>(userId)
                     .Updated<BranchOffice>(branchOfficeId)
                     .Complete();
            }
        }
    }
}
