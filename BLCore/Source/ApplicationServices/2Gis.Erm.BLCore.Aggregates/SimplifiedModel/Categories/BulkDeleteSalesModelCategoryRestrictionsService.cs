using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories
{
    public sealed class BulkDeleteSalesModelCategoryRestrictionsService : IBulkDeleteSalesModelCategoryRestrictionsService
    {
        private readonly IRepository<SalesModelCategoryRestriction> _repository;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkDeleteSalesModelCategoryRestrictionsService(IRepository<SalesModelCategoryRestriction> repository, IOperationScopeFactory scopeFactory)
        {
            _repository = repository;
            _scopeFactory = scopeFactory;
        }

        public void Delete(IEnumerable<SalesModelCategoryRestriction> recordsToDelete)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkDeleteIdentity, SalesModelCategoryRestriction>())
            {
                _repository.DeleteRange(recordsToDelete);
                _repository.Save();

                scope.Deleted(recordsToDelete)
                     .Complete();
            }
        }
    }
}