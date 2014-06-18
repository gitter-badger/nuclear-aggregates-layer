using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class CreateFirmAggregateService : IAggregatePartRepository<Firm>, ICreateAggregateRepository<Firm>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Firm> _firmRepository;

        public CreateFirmAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Firm> firmRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmRepository = firmRepository;
        }

        public int Create(Firm entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Firm>())
            {
                _firmRepository.Add(entity);
                operationScope.Added<Firm>(entity.Id);

                var count = _firmRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}