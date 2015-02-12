using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public class CategoryReadModel : ICategoryReadModel
    {
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

        public IDictionary<long, IEnumerable<long>> GetFirmAddressesCategories(long destOrganizationUnitId, IEnumerable<long> firmAddressIds)
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

            return directLinkedCategories.Union(firstLevelCategories).GroupBy(x => x.FirmAddressId).ToDictionary(x => x.Key, x => x.Select(y => y.Category.Id));
        }

        public IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetFirmCategories(IEnumerable<long> firmCategoryIds, SalesModel salesModel, long organizationUnitId)
        {
            return _finder.Find(Specs.Find.ByIds<Category>(firmCategoryIds)
                                && CategorySpecs.Categories.Find.ActiveCategoryForSalesModelInOrganizationUnit(salesModel, organizationUnitId))
                          .Select(item => new LinkingObjectsSchemaDto.CategoryDto { Id = item.Id, Name = item.Name, Level = item.Level, })
                          .Distinct()
                          .ToArray();
        }

        public IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetAdditionalCategories(IEnumerable<long> firmCategoryIds, long orderPositionId, SalesModel salesModel, long organizationUnitId)
        {
            return _finder.Find(OrderSpecs.OrderPositionAdvertisements.Find.ByOrderPosition(orderPositionId))
                          .Where(opa => opa.CategoryId.HasValue)
                          .Select(opa => opa.Category)
                          .Where(CategorySpecs.Categories.Find.ActiveCategoryForSalesModelInOrganizationUnit(salesModel, organizationUnitId))
                          .Where(category => !firmCategoryIds.Contains(category.Id))
                          .Select(category => new LinkingObjectsSchemaDto.CategoryDto { Id = category.Id, Name = category.Name, Level = category.Level, })
                          .Distinct()
                          .ToArray();
        }

        public IDictionary<long, string> PickCategoriesUnsupportedBySalesModelInOrganizationUnit(SalesModel salesModel, long destOrganizationUnitId, IEnumerable<long> categoryIds)
        {
            var allowedCategoriesSpecification = CategorySpecs.Categories.Find.ActiveCategoryForSalesModelInOrganizationUnit(salesModel, destOrganizationUnitId);
            return _finder.Find(Specs.Find.ByIds<Category>(categoryIds) && !allowedCategoriesSpecification)
                          .ToDictionary(category => category.Id, category => category.Name);
        }
    }
}