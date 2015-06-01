using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkUpdateCategoryFirmAddressAggregateService : IBulkUpdateCategoryFirmAddressAggregateService
    {
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkUpdateCategoryFirmAddressAggregateService(IRepository<CategoryFirmAddress> categoryFirmAddressRepository,
                                                             IOperationScopeFactory operationScopeFactory)
        {
            _categoryFirmAddressRepository = categoryFirmAddressRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Update(IReadOnlyCollection<CategoryFirmAddress> categoryFirmAddresses)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<BulkUpdateIdentity, CategoryFirmAddress>())
            {
                foreach (var categoryFirmAddress in categoryFirmAddresses)
                {
                    _categoryFirmAddressRepository.Update(categoryFirmAddress);
                    scope.Updated(categoryFirmAddress);
                }

                _categoryFirmAddressRepository.Save();
                scope.Complete();
            }
        }
    }
}