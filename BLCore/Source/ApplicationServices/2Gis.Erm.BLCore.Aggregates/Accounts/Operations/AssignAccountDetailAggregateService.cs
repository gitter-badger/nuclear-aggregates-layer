using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public class AssignAccountDetailAggregateService : IAssignAccountDetailAggregateService
    {
        private readonly ISecureRepository<AccountDetail> _accountDetailGenericSecureRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AssignAccountDetailAggregateService(ISecureRepository<AccountDetail> accountDetailGenericSecureRepository, IOperationScopeFactory scopeFactory)
        {
            _accountDetailGenericSecureRepository = accountDetailGenericSecureRepository;
            _scopeFactory = scopeFactory;
        }

        public IEnumerable<ChangesDescriptor> Assign(AccountDetail entity, long ownerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, AccountDetail>())
            {
                var originalOwnerCode = entity.OwnerCode;
                entity.OwnerCode = ownerCode;
                _accountDetailGenericSecureRepository.Update(entity);
                _accountDetailGenericSecureRepository.Save();

                operationScope
                    .Updated<AccountDetail>(entity.Id)
                    .Complete();

                return new[] { ChangesDescriptor.Create(entity, x => x.OwnerCode, originalOwnerCode, entity.OwnerCode) };
            }
        }
    }
}