﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public class CategoryReadModel : ICategoryReadModel
    {
        private const long DefaultCategoryGroupId = 3;

        private readonly IFinder _finder;

        public CategoryReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public string GetCategoryName(long categoryId)
        {
            return _finder.Find(Specs.Find.ById<Category>(categoryId)).Select(x => x.Name).Single();
        }

        public IReadOnlyDictionary<long, int> GetCategoryLevels(IEnumerable<long> categoryIds)
        {
            return _finder.Find(Specs.Find.ByIds<Category>(categoryIds)).ToDictionary(x => x.Id, x => x.Level);
        }

        public IDictionary<long, IEnumerable<LinkingObjectsSchemaCategoryDto>> GetFirmAddressesCategories(long destOrganizationUnitId, IEnumerable<long> firmAddressIds)
        {
            var categoryOrganizationUnits =
                _finder.Find(CategorySpecs.CategoryOrganizationUnits.Find.ForOrganizationUnit(destOrganizationUnitId) &&
                             Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>());

            var categoryFirmAddress = _finder.Find(CategorySpecs.CategoryFirmAddresses.Find.ByFirmAddresses(firmAddressIds) &&
                                                   Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>());

            var directLinkedCategories = categoryFirmAddress.Join(categoryOrganizationUnits,
                                                                  categoryAddress => categoryAddress.CategoryId,
                                                                  categoryOrganizationUnit => categoryOrganizationUnit.CategoryId,
                                                                  (x, y) => new
                                                                  {
                                                                      x.FirmAddressId,
                                                                      x.Category
                                                                  })
                                                            .Where(x => x.Category.IsActive && !x.Category.IsDeleted);

            var firstLevelCategories = directLinkedCategories.Where(x => x.Category.Level == 3)
                                                             .Select(x => new { x.FirmAddressId, Category = x.Category.ParentCategory.ParentCategory })
                                                             .Where(x => x.Category.IsActive && !x.Category.IsDeleted);

            return directLinkedCategories.Union(firstLevelCategories)
                                         .GroupBy(x => x.FirmAddressId)
                                         .ToDictionary(x => x.Key,
                                                       x => x.Select(y => new LinkingObjectsSchemaCategoryDto
                                                                              {
                                                                                  Id = y.Category.Id,
                                                                                  Level = y.Category.Level,
                                                                                  Name = y.Category.Name
                                                                              }));
        }

        public IEnumerable<LinkingObjectsSchemaCategoryDto> GetFirmCategories(IEnumerable<long> firmCategoryIds, SalesModel salesModel, long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ByIds<Category>(firmCategoryIds)
                                && CategorySpecs.Categories.Find.ActiveCategoryForSalesModelInOrganizationUnit(salesModel, organizationUnitId))
                          .Select(item => new LinkingObjectsSchemaCategoryDto { Id = item.Id, Name = item.Name, Level = item.Level, })
                          .Distinct()
                          .ToArray();
        }

        public IEnumerable<CategoryAsLinkingObjectDto> GetSalesIntoCategories(long orderPositionId)
        {
            return _finder.Find(OrderSpecs.OrderPositionAdvertisements.Find.ByOrderPosition(orderPositionId))
                          .Where(opa => opa.CategoryId.HasValue)
                          .Select(opa => new CategoryAsLinkingObjectDto
                                             {
                                                 CategoryId = opa.CategoryId.Value,
                                                 CategoryName = opa.Category.Name,
                                                 CategoryLevel = opa.Category.Level,
                                                 FirmAddressId = opa.FirmAddressId,
                                                 PositionId = opa.PositionId
                                             })
                          .ToArray();
        }

        public IDictionary<long, string> PickCategoriesUnsupportedBySalesModelInOrganizationUnit(SalesModel salesModel, long destOrganizationUnitId, IEnumerable<long> categoryIds)
        {
            var allowedCategoriesSpecification = CategorySpecs.Categories.Find.ActiveCategoryForSalesModelInOrganizationUnit(salesModel, destOrganizationUnitId);
            return _finder.Find(Specs.Find.ByIds<Category>(categoryIds) && !allowedCategoriesSpecification)
                          .ToDictionary(category => category.Id, category => category.Name);
        }

        public IEnumerable<CategoryGroupDto> GetCategoryGroups()
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryGroup>())
                          .OrderBy(x => x.GroupRate)
                          .Select(group => new CategoryGroupDto
                                               {
                                                   Id = group.Id,
                                                   Name = group.CategoryGroupName,
                                                   IsDefault = group.Id == DefaultCategoryGroupId
                                               })
                          .ToArray();
        }

        public IEnumerable<CategoryGroupMembershipDto> GetCategoryGroupMembership(long organizationUnitId)
        {
            var filter = Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>()
                         && CategorySpecs.CategoryOrganizationUnits.Find.ForOrganizationUnit(organizationUnitId)
                         && CategorySpecs.CategoryOrganizationUnits.Find.ForActiveAndNotDeletedCategory();

            var selector = CategorySpecs.CategoryOrganizationUnits.Select.CategoryGroupMembershipDto();

            return _finder.Find(selector, filter)
                          .Where(dto => dto.CategoryLevel == 3)
                          .ToArray();
        }

        public IEnumerable<CategoryOrganizationUnitDto> GetCategoryOrganizationUnits(IEnumerable<long> ids)
        {
            return _finder.Find(Specs.Find.ByIds<CategoryOrganizationUnit>(ids))
                                          .Select(unit => new CategoryOrganizationUnitDto { Unit = unit, Level = unit.Category.Level })
                                          .ToArray();
        }
    }
}