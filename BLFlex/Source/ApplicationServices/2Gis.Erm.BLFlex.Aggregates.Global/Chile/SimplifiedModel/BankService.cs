using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel
{
    public class BankService : IBankService
    {
        private readonly IRepository<Bank> _bankRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BankService(IRepository<Bank> bankRepository, IIdentityProvider identityProvider, IOperationScopeFactory operationScopeFactory)
        {
            _bankRepository = bankRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(Bank bank)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Bank>())
            {
                _identityProvider.SetFor(bank);
                _bankRepository.Add(bank);
                operationScope.Added<Bank>(bank.Id);

                var count = _bankRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Update(Bank bank)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Bank>())
            {
                _bankRepository.Update(bank);
                operationScope.Updated<Bank>(bank.Id);

                var count = _bankRepository.Save();

                operationScope.Complete();

                return count;
            }
        }

        public int Delete(Bank bank)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Bank>())
            {
                _bankRepository.Delete(bank);
                operationScope.Deleted<Bank>(bank.Id);

                var count = _bankRepository.Save();
                
                operationScope.Complete();
                return count;
            }
        }
    }
}
