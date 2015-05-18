using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkCreateCategoryFirmAddressAggregateService : IBulkCreateCategoryFirmAddressAggregateService
    {
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateCategoryFirmAddressAggregateService(IRepository<CategoryFirmAddress> categoryFirmAddressRepository,
                                                             IIdentityProvider identityProvider,
                                                             IOperationScopeFactory operationScopeFactory)
        {
            _categoryFirmAddressRepository = categoryFirmAddressRepository;
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(IReadOnlyCollection<CategoryFirmAddress> categoryFirmAddresses)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkCreateIdentity, CategoryFirmAddress>())
            {
                _identityProvider.SetFor(categoryFirmAddresses);
                _categoryFirmAddressRepository.AddRange(categoryFirmAddresses);
                _categoryFirmAddressRepository.Save();

                scope.Added(categoryFirmAddresses.AsEnumerable());
                scope.Complete();
            }
        }
    }
}