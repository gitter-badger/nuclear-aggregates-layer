﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteCategoryGroupService : IDeleteGenericEntityService<CategoryGroup>
    {
        private readonly IFinder _finder;
        private readonly ICategoryService _categoryService;

        public DeleteCategoryGroupService(ICategoryService categoryService, IFinder finder)
        {
            _categoryService = categoryService;
            _finder = finder;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var categoryGroupDto = GetCategoryGroupDto(entityId);

            if (categoryGroupDto == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            _categoryService.Delete(categoryGroupDto.Group);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var categoryGroupDto = GetCategoryGroupDto(entityId);

            if (categoryGroupDto == null)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.EntityNotFound
                    };
            }

            if (categoryGroupDto.CategoriesCount > 0)
            {
                return new DeleteConfirmationInfo
                    {
                        DeleteConfirmation = "TTT",
                        IsDeleteAllowed = true,
                    };
            }

            return null;
        }

        private CategoryGroupWithLinkedCategoriesCountDto GetCategoryGroupDto(long categoryGroup)
        {
            return _finder.Find<CategoryGroup>(x => x.Id == categoryGroup)
                          .Select(
                              x =>
                              new CategoryGroupWithLinkedCategoriesCountDto
                              {
                                  Group = x,
                                  CategoriesCount = x.CategoryOrganizationUnits.Count()
                              })
                          .FirstOrDefault();
        }

        private sealed class CategoryGroupWithLinkedCategoriesCountDto
        {
            public CategoryGroup Group { get; set; }
            public int CategoriesCount { get; set; }
        }
    }
}