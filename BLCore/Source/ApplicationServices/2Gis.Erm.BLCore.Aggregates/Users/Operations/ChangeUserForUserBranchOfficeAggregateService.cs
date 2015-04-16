using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Users.Operations
{
    public class ChangeUserForUserBranchOfficeAggregateService : IChangeUserForUserBranchOfficeAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<UserBranchOffice> _repository;

        public ChangeUserForUserBranchOfficeAggregateService(IOperationScopeFactory operationScopeFactory, IRepository<UserBranchOffice> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void ChangeUser(IEnumerable<UserBranchOffice> userBranchOffices, long userCode)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, UserBranchOffice>())
            {
                foreach (var userBranchOffice in userBranchOffices)
                {
                    userBranchOffice.UserId = userCode;
                    _repository.Update(userBranchOffice);
                    scope.Updated(userBranchOffice);
                }

                _repository.Save();

                scope.Complete();
            }
        }
    }
}