using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class CreateCategoryFirmAddressAggregateService : IAggregatePartRepository<Firm>, ICreateAggregateRepository<CategoryFirmAddress>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressRepository;
        private readonly IIdentityProvider _identityProvider;

        public CreateCategoryFirmAddressAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<CategoryFirmAddress> categoryFirmAddressRepository,
            IIdentityProvider identityProvider)
        {
            _operationScopeFactory = operationScopeFactory;
            _categoryFirmAddressRepository = categoryFirmAddressRepository;
            _identityProvider = identityProvider;
        }

        public int Create(CategoryFirmAddress entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, CategoryFirmAddress>())
            {
                _identityProvider.SetFor(entity);
                _categoryFirmAddressRepository.Add(entity);
                operationScope.Added<CategoryFirmAddress>(entity.Id);

                var count = _categoryFirmAddressRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}