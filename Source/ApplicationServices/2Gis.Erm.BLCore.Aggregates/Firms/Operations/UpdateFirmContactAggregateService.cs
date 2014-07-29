using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class UpdateFirmContactAggregateService : IAggregatePartRepository<Firm>, IUpdateAggregateRepository<FirmContact>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmContact> _firmContactRepository;

        public UpdateFirmContactAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmContact> firmContactRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmContactRepository = firmContactRepository;
        }

        public int Update(FirmContact entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, FirmContact>())
            {
                _firmContactRepository.Update(entity);
                operationScope.Updated<FirmContact>(entity.Id);

                var count = _firmContactRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}