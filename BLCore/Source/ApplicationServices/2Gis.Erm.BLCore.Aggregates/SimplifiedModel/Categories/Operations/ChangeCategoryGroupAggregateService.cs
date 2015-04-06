using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories.Operations
{
    public class ChangeCategoryGroupAggregateService : IChangeCategoryGroupAggregateService
    {
        private const long DefaultCategoryGroupId = 3;

        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IRepository<CategoryOrganizationUnit> _categoryOrganizationUnitRepository;

        public ChangeCategoryGroupAggregateService(IOperationScopeFactory scopeFactory, 
            IRepository<CategoryOrganizationUnit> categoryOrganizationUnitRepository)
        {
            _scopeFactory = scopeFactory;
            _categoryOrganizationUnitRepository = categoryOrganizationUnitRepository;
        }

        public void Change(IEnumerable<CategoryOrganizationUnitDto> dtos, IDictionary<long, long?> categoryToGroupMapping)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, CategoryOrganizationUnit>())
            {
                foreach (var record in dtos)
                {
                    var newGroupId = categoryToGroupMapping[record.Unit.Id] ?? DefaultCategoryGroupId;
                    if (record.Level != 3 && newGroupId != DefaultCategoryGroupId)
                    {
                        throw new BusinessLogicException(BLResources.CanNotChangeGroupForUpperLevelsCategory);
                    }

                    if (record.Level != 3)
                    {
                        record.Unit.CategoryGroupId = null;
                    }
                    else
                    {
                        record.Unit.CategoryGroupId = newGroupId;
                    }

                    _categoryOrganizationUnitRepository.Update(record.Unit);
                    scope.Updated(record.Unit);
                }

                _categoryOrganizationUnitRepository.Save();
                scope.Complete();
            }
        }
    }
}