using System;
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

        [Obsolete]
        private readonly IReadOnlyDictionary<long, IReadOnlyCollection<long>> _supportedCategoryOrganizationUnits =
            new Dictionary<long, IReadOnlyCollection<long>>
                {
                    {
                        6,          // Новосибирск:
                        new long[]
                            {
                                192,    // Кинотеатры
                                356,    // Магазины обувные
                                390,    // Часы
                                15502,  // Товары для творчества и рукоделия
                                207,    // Аптеки
                                526,    // Сумки / кожгалантерея
                                405,    // Автомойки
                                508,    // Ткани
                                384,    // Охотничьи принадлежности / Аксессуары
                                347,    // Книги
                                205     // Ветеринарные клиники
                            }
                    },
                    {
                        1,          // Самара:
                        new long[]
                            {
                                13100,  // Детская обувь 
                                609,    // Детская одежда
                                346,    // Игрушки 
                                345,    // Товары для новорожденных 
                                533,    // Заказ пассажирского легкового транспорта
                                207,    // Аптеки 
                                1203,   // Доставка готовых блюд
                                161,    // Кафе
                                164,    // Рестораны
                                15791,  // Суши-бары / рестораны
                            }
                    },
                    {
                        2,          // Екатеринбург:
                        new long[]
                            {
                                14426,  // Верхняя одежда 
                                354,    // Головные уборы
                                606,    // Женская одежда
                                355,    // Меха/Дублёнки/Кожа
                                612,    // Мужская одежда 
                                383,    // Свадебные товары 
                                207,    // Аптеки
                            }
                    }
                };

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

        public IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetFirmCategories(IEnumerable<long> firmCategoryIds)
        {
            return _finder.Find(Specs.Find.ByIds<Category>(firmCategoryIds))
                          .Select(item => new LinkingObjectsSchemaDto.CategoryDto { Id = item.Id, Name = item.Name, Level = item.Level, })
                          .Distinct()
                          .ToArray();
        }

        public IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetAdditionalCategories(IEnumerable<long> firmCategoryIds, long orderPositionId)
        {
            return _finder.Find(OrderSpecs.OrderPositionAdvertisements.Find.ByOrderPosition(orderPositionId))
                          .Where(opa => opa.CategoryId.HasValue)
                          .Select(opa => opa.Category)
                          .Where(category => !firmCategoryIds.Contains(category.Id))
                          .Select(category => new LinkingObjectsSchemaDto.CategoryDto { Id = category.Id, Name = category.Name, Level = category.Level, })
                          .Distinct()
                          .ToArray();
        }

        public IEnumerable<long> GetCategoriesSupportedBySalesModelInOrganizationUnit(SalesModel salesModel, long organizationUnitId)
        {
            var supportedCategoryIds = salesModel == SalesModel.PlannedProvision
                                           ? _supportedCategoryOrganizationUnits.ContainsKey(organizationUnitId)
                                                 ? _supportedCategoryOrganizationUnits[organizationUnitId]
                                                 : Enumerable.Empty<long>()
                                           : _finder.Find(Specs.Find.ActiveAndNotDeleted<Category>()).Select(x => x.Id).ToArray();

            return _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>() &&
                                CategorySpecs.CategoryOrganizationUnits.Find.ForOrganizationUnit(organizationUnitId) &&
                                CategorySpecs.CategoryOrganizationUnits.Find.ForCategories(supportedCategoryIds))
                          .Select(x => x.CategoryId)
                          .ToArray();
        }

        public IDictionary<long, string> PickCategoriesUnsupportedBySalesModelInOrganizationUnit(SalesModel salesModel, long destOrganizationUnitId, IEnumerable<long> categoryIds)
        {
            var allowedCategories = GetCategoriesSupportedBySalesModelInOrganizationUnit(salesModel, destOrganizationUnitId);
            var deniedCategories = categoryIds.Where(x => !allowedCategories.Contains(x));
            return _finder.Find(Specs.Find.ByIds<Category>(deniedCategories))
                          .ToDictionary(category => category.Id, category => category.Name);
        }
    }
}