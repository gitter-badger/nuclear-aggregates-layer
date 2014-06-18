using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class UpdateFirmAddressAggregateService : IAggregatePartRepository<Firm>, IUpdateAggregateRepository<FirmAddress>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmAddress> _firmAddressRepository;

        public UpdateFirmAddressAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmAddress> firmAddressRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmAddressRepository = firmAddressRepository;
        }

        public int Update(FirmAddress entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
            {
                _firmAddressRepository.Update(entity);
                operationScope.Updated<FirmAddress>(entity.Id);

                var count = _firmAddressRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}