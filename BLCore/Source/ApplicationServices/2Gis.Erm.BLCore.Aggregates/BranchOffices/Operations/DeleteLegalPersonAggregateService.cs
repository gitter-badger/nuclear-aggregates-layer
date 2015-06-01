using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.Operation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class DeleteUserBranchOfficesAggregateService : IDeleteUserBranchOfficesAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<UserBranchOffice> _repository;

        public DeleteUserBranchOfficesAggregateService(IOperationScopeFactory operationScopeFactory, IRepository<UserBranchOffice> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Delete(IEnumerable<UserBranchOffice> userBranchOffices)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, UserBranchOffice>())
            {
                foreach (var userBranchOffice in userBranchOffices)
                {
                    _repository.Delete(userBranchOffice);
                    operationScope.Deleted(userBranchOffice);
                }

                _repository.Save();
                operationScope.Complete();
            }
        }
    }
}