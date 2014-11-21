﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class BulkDeleteCategoryFirmAddressAggregateService : IBulkDeleteCategoryFirmAddressAggregateService
    {
        private readonly IRepository<CategoryFirmAddress> _categoryFirmAddressRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkDeleteCategoryFirmAddressAggregateService(IRepository<CategoryFirmAddress> categoryFirmAddressRepository,
                                                             IOperationScopeFactory operationScopeFactory)
        {
            _categoryFirmAddressRepository = categoryFirmAddressRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Delete(IReadOnlyCollection<CategoryFirmAddress> categoryFirmAddresses)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, CategoryFirmAddress>())
            {
                _categoryFirmAddressRepository.DeleteRange(categoryFirmAddresses);
                _categoryFirmAddressRepository.Save();

                scope.Deleted(categoryFirmAddresses.AsEnumerable())
                     .Complete();
            }
        }
    }
}