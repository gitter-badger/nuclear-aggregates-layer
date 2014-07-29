using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class CreateFirmAddressAggregateService : IAggregatePartRepository<Firm>, ICreateAggregateRepository<FirmAddress>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmAddress> _firmAddressRepository;

        public CreateFirmAddressAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmAddress> firmAddressRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmAddressRepository = firmAddressRepository;
        }

        public int Create(FirmAddress entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, FirmAddress>())
            {
                _firmAddressRepository.Add(entity);
                operationScope.Added<FirmAddress>(entity.Id);

                var count = _firmAddressRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}