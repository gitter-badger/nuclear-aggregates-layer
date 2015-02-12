using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories
{
    public sealed class BulkCreateSalesModelCategoryRestrictionsService : IBulkCreateSalesModelCategoryRestrictionsService
    {
        private readonly IRepository<SalesModelCategoryRestriction> _repository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public BulkCreateSalesModelCategoryRestrictionsService(IIdentityProvider identityProvider,
                                                               IOperationScopeFactory scopeFactory,
                                                               IRepository<SalesModelCategoryRestriction> repository)
        {
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _repository = repository;
        }

        public void Create(IEnumerable<SalesModelCategoryRestriction> recordsToCreate)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkCreateIdentity, SalesModelCategoryRestriction>())
            {
                _identityProvider.SetFor(recordsToCreate.ToArray());
                _repository.AddRange(recordsToCreate);
                _repository.Save();

                scope.Added(recordsToCreate)
                     .Complete();
            }
        }
    }
}