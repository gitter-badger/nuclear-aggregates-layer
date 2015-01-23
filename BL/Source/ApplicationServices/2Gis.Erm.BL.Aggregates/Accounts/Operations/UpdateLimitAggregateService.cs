using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Aggregates.Accounts.Operations
{
    public sealed class UpdateLimitAggregateService : IUpdateLimitAggregateService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureRepository<Limit> _limitGenericSecureRepository;

        public UpdateLimitAggregateService(IOperationScopeFactory scopeFactory, ISecureRepository<Limit> limitGenericSecureRepository)
        {
            _scopeFactory = scopeFactory;
            _limitGenericSecureRepository = limitGenericSecureRepository;
        }

        public void Update(Limit limit, long accountOwnerCode)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Limit>())
            {
                // TODO {all, 28.10.2014}: есть подозрение, что код куратора лимита должен выставляться не здесь
                limit.OwnerCode = accountOwnerCode;

                _limitGenericSecureRepository.Update(limit);
                _limitGenericSecureRepository.Save();

                operationScope
                    .Updated<Limit>(limit.Id)
                    .Complete();
            }
        }
    }
}
