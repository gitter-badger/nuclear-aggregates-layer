using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories
{
    public sealed class BulkDeleteSalesModelCategoryRestrictionsService : IBulkDeleteSalesModelCategoryRestrictionsService
    {
        private readonly IBatchDeletePersistenceService _batchDeletePersistenceService;

        public BulkDeleteSalesModelCategoryRestrictionsService(IBatchDeletePersistenceService batchDeletePersistenceService)
        {
            _batchDeletePersistenceService = batchDeletePersistenceService;
        }

        public void Delete(IEnumerable<SalesModelCategoryRestriction> recordsToDelete)
        {
            _batchDeletePersistenceService.Delete(recordsToDelete);
        }
    }
}