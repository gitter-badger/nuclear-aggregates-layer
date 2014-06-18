using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class UpdateCategoryFirmAddressAggregateService : IAggregatePartRepository<Firm>, IUpdateAggregateRepository<CategoryFirmAddress>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressRepository;

        public UpdateCategoryFirmAddressAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<CategoryFirmAddress> categoryFirmAddressRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _categoryFirmAddressRepository = categoryFirmAddressRepository;
        }

        public int Update(CategoryFirmAddress entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, CategoryFirmAddress>())
            {
                _categoryFirmAddressRepository.Update(entity);
                operationScope.Updated<CategoryFirmAddress>(entity.Id);

                var count = _categoryFirmAddressRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}