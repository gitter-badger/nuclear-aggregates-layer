using DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Aggregates.Accounts.Operations
{
    public sealed class DeleteLimitAggregateService : IDeleteLimitAggregateService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecureRepository<Limit> _limitGenericSecureRepository;

        public DeleteLimitAggregateService(IOperationScopeFactory scopeFactory, ISecureRepository<Limit> limitGenericSecureRepository)
        {
            _scopeFactory = scopeFactory;
            _limitGenericSecureRepository = limitGenericSecureRepository;
        }

        public void Delete(Limit limit)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Limit>())
            {
                _limitGenericSecureRepository.Delete(limit);
                _limitGenericSecureRepository.Save();

                operationScope
                    .Deleted<Limit>(limit.Id)
                    .Complete();
            }
        }
    }
}
