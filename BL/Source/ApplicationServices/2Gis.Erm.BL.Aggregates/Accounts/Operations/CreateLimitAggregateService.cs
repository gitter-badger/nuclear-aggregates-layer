using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Aggregates.Accounts.Operations
{
    public sealed class CreateLimitAggregateService : ICreateLimitAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureRepository<Limit> _limitGenericSecureRepository;

        public CreateLimitAggregateService(IIdentityProvider identityProvider, IOperationScopeFactory scopeFactory, ISecureRepository<Limit> limitGenericSecureRepository)
        {
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _limitGenericSecureRepository = limitGenericSecureRepository;
        }

        public void Create(Limit limit, long accountOwnerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, Limit>())
            {
                _identityProvider.SetFor(limit);

                // TODO {all, 28.10.2014}: есть подозрение, что код куратора лимита должен выставляться не здесь
                limit.OwnerCode = accountOwnerCode;

                _limitGenericSecureRepository.Add(limit);
                _limitGenericSecureRepository.Save();

                operationScope
                    .Added<Limit>(limit.Id)
                    .Complete();
            }
        }
    }
}
