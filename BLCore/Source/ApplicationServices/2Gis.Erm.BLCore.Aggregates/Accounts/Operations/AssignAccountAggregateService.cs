using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public class AssignAccountAggregateService : IAssignAccountAggregateService
    {
        private readonly ISecureRepository<Account> _accountGenericSecureRepository;
        private readonly ISecureRepository<Limit> _limitGenericSecureRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AssignAccountAggregateService(ISecureRepository<Account> accountGenericSecureRepository,
                                             ISecureRepository<Limit> limitGenericSecureRepository,
                                             IOperationScopeFactory scopeFactory)
        {
            _accountGenericSecureRepository = accountGenericSecureRepository;
            _limitGenericSecureRepository = limitGenericSecureRepository;
            _scopeFactory = scopeFactory;
        }

        public void Assign(AssignAccountDto assignAccountDto, long ownerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Account>())
            {
                foreach (var limit in assignAccountDto.Limits)
                {
                    limit.OwnerCode = ownerCode;
                    _limitGenericSecureRepository.Update(limit);

                    operationScope.Updated(limit);
                }

                _limitGenericSecureRepository.Save();

                var account = assignAccountDto.Account;
                account.OwnerCode = ownerCode;
                _accountGenericSecureRepository.Update(account);
                _accountGenericSecureRepository.Save();

                operationScope
                    .Updated(account)
                    .Complete();
            }
        }
    }
}