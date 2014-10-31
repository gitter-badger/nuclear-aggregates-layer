using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
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
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity>(EntityName.Limit))
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
