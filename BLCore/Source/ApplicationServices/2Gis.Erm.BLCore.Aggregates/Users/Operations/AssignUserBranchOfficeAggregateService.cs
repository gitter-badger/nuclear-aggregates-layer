using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Users.Operations
{
    public class AssignUserBranchOfficeAggregateService : IAssignUserBranchOfficeAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<UserBranchOffice> _repository;

        public AssignUserBranchOfficeAggregateService(IOperationScopeFactory operationScopeFactory, IRepository<UserBranchOffice> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Assign(IEnumerable<UserBranchOffice> userBranchOffices, long ownerCode)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, UserBranchOffice>())
            {
                foreach (var userBranchOffice in userBranchOffices)
                {
                    userBranchOffice.UserId = ownerCode;
                    _repository.Update(userBranchOffice);
                    scope.Updated(userBranchOffice);
                }

                _repository.Save();

                scope.Complete();
            }
        }
    }
}