using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Category;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Categories
{
    public sealed class ChangeCategoryGroupService : IChangeCategoryGroupService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICategoryReadModel _categoryReadModel;
        private readonly IChangeCategoryGroupAggregateService _aggregateService;

        public ChangeCategoryGroupService(IOperationScopeFactory scopeFactory, ICategoryReadModel categoryReadModel, IChangeCategoryGroupAggregateService aggregateService)
        {
            _scopeFactory = scopeFactory;
            _categoryReadModel = categoryReadModel;
            _aggregateService = aggregateService;
        }

        public void SetCategoryGroupMembership(IEnumerable<CategoryGroupMembershipDto> membership)
        {
            var categoryToGroupMapping = membership.ToDictionary(dto => dto.Id, dto => dto.CategoryGroupId);

            using (var scope = _scopeFactory.CreateNonCoupled<ChangeCategoryGroupIdentity>())
            {
                var dtos = _categoryReadModel.GetCategoryOrganizationUnits(categoryToGroupMapping.Keys);

                if (!dtos.Any())
                {
                    return;
                }

                _aggregateService.Change(dtos, categoryToGroupMapping);
                scope.Complete();
            }
        }
    }
}