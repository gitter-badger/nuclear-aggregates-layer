using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Categories
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly IRepository<CategoryGroup> _categoryGroupRepository;
        private readonly IRepository<CategoryOrganizationUnit> _categoryOrganizationUnitRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public CategoryService(IFinder finder,
                               IRepository<Category> categoryRepository,
                               IRepository<CategoryOrganizationUnit> categoryOrganizationUnitRepository,
                               IRepository<CategoryGroup> categoryGroupRepository,
                               IIdentityProvider identityProvider,
                               IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _categoryRepository = categoryRepository;
            _categoryOrganizationUnitRepository = categoryOrganizationUnitRepository;
            _categoryGroupRepository = categoryGroupRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void CreateOrUpdate(CategoryGroup entity)
        {
            if (entity.IsNew())
            {
                _categoryGroupRepository.Add(entity);
            }
            else
            {
                _categoryGroupRepository.Update(entity);
            }

            _categoryGroupRepository.Save();
        }

        public void FixAffectedCategories(int level, CategoryImportContext context)
        {
            // Исправление категорий одного уровня влияет на категории родительского уровня, 
            // поэтому после исправления соотвествующие категории родительского уровня нужно пометить на исправление.
            // возможность для дальнейшей оптимизации: учитывать только те категории, в которых реально произошли изменения.
            var categoryIds = context.GetAffectedCategories(level);
            foreach (var categoryId in categoryIds)
            {
                FixAffectedCategory(categoryId);
            }

            _categoryOrganizationUnitRepository.Save();

            var parentIds = ResolveParentIds(categoryIds);
            foreach (var id in parentIds)
            {
                context.PutAffectedCategory(level - 1, id);
            }
        }

        public void SetCategoryGroupMembership(long organizationUnitId, IEnumerable<CategoryGroupMembershipDto> membership)
        {
            if (!membership.Any())
            {
                return;
            }

            var categoryOrgUnitIds = membership.Select(x => x.Id).Distinct().ToArray();
            var categoryOrgUnits = _finder.Find<CategoryOrganizationUnit>(x => categoryOrgUnitIds.Contains(x.Id)).ToDictionary(x => x.Id, y => y);

            if (categoryOrgUnits.Count == 0)
            {
                return;
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var categoryGroupMembershipDto in membership)
                {
                    CategoryOrganizationUnit categoryOrgUnitToUpdate;
                    if (categoryOrgUnits.TryGetValue(categoryGroupMembershipDto.Id, out categoryOrgUnitToUpdate))
                    {
                        categoryOrgUnitToUpdate.CategoryGroupId = categoryGroupMembershipDto.CategoryGroupId;
                        _categoryOrganizationUnitRepository.Update(categoryOrgUnitToUpdate);
                    }
                }

                _categoryOrganizationUnitRepository.Save();
                scope.Complete();
            }
        }

        public void Delete(CategoryGroup categoryGroup)
        {
            var linkedCategories = _finder.Find<CategoryOrganizationUnit>(x => x.CategoryGroupId == categoryGroup.Id).ToArray();

            using (var scope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var category in linkedCategories)
                {
                    category.CategoryGroupId = null;
                    _categoryOrganizationUnitRepository.Update(category);
                }

                _categoryOrganizationUnitRepository.Save();
                _categoryGroupRepository.Delete(categoryGroup);
                _categoryGroupRepository.Save();
                scope.Complete();
            }
        }

        public void ImportOrganizationUnits(IEnumerable<RubricServiceBusDto> categories, CategoryImportContext context)
        {
            foreach (var dto in categories)
            {
                var convertedIds = ResolveOrganizatuionUnitsDgppIds(dto.OrganizationUnitsDgppIds, context);
                UpdateCategoryOrganizationUnits(dto.Id, convertedIds);
                if (dto.ParentId.HasValue)
                {
                    var parentId = dto.ParentId.Value;
                    context.PutAffectedCategory(dto.Level - 1, parentId);
                }
            }

            _categoryOrganizationUnitRepository.Save();
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
                _finder.Find(CategorySpecifications.CategoryOrganizationUnits.Find.ForOrganizationUnit(destOrganizationUnitId) &&
                             Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>());

            var categoryFirmAddress = _finder.Find(CategorySpecifications.CategoryFirmAddresses.Find.ByFirmAddresses(firmAddressIds) &&
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
                                           ? SupportedCategoryOrganizationUnits.ContainsKey(organizationUnitId)
                                                 ? SupportedCategoryOrganizationUnits[organizationUnitId]
                                                 : Enumerable.Empty<long>()
                                           : _finder.Find(Specs.Find.ActiveAndNotDeleted<Category>()).Select(x => x.Id).ToArray();

            return _finder.Find(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>() &&
                                CategorySpecifications.CategoryOrganizationUnits.Find.ForOrganizationUnit(organizationUnitId) &&
                                CategorySpecifications.CategoryOrganizationUnits.Find.ForCategories(supportedCategoryIds))
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

        private IEnumerable<long> ResolveParentIds(IEnumerable<long> categoryIds)
        {
            return _finder.Find<Category>(category => categoryIds.Contains(category.Id))
                          .Where(category => category.ParentId.HasValue)
                          .Select(category => category.ParentId.Value)
                          .Distinct()
                          .ToArray();
        }

        private void FixAffectedCategory(long categoryId)
        {
            var organizationUnits = _finder.Find(Specs.Find.ActiveAndNotDeleted<Category>())
                                           .Where(category => category.ParentId == categoryId)
                                           .SelectMany(category => category.CategoryOrganizationUnits)
                                           .Where(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                           .Select(link => link.OrganizationUnitId)
                                           .Distinct();

            UpdateCategoryOrganizationUnits(categoryId, organizationUnits);
        }

        private void UpdateCategoryOrganizationUnits(long categoryId, IEnumerable<long> organizationUnitIds)
        {
            // В оригинальном коде использовался такой-же подход, возможно от того, что при удалении записи только помечаются удалёнными, 
            // этот подход позволяет избежать дублирования одинаковых записей, различающихся флагом удалённости.
            var categoryUnits = _finder.Find<CategoryOrganizationUnit>(link => link.CategoryId == categoryId);

            // Записи, которые должны быть, которые есть, но помечены удаленными.
            var linksToRestore = categoryUnits.Where(link => organizationUnitIds.Contains(link.OrganizationUnitId) && (!link.IsActive || link.IsDeleted));
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, CategoryOrganizationUnit>())
            {
                foreach (var link in linksToRestore)
                {
                    link.IsActive = true;
                    link.IsDeleted = false;
                    _categoryOrganizationUnitRepository.Update(link);
                    scope.Updated<CategoryOrganizationUnit>(link.Id);
                }

                scope.Complete();
            }

            // Записи, которые должны быть, которых нет совсем
            var linksToCreate = organizationUnitIds.Except(categoryUnits.Select(link => link.OrganizationUnitId));
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, CategoryOrganizationUnit>())
            {
                foreach (var organizationUnitId in linksToCreate)
                {
                    var categoryOrganizationUnit = new CategoryOrganizationUnit
                        {
                            CategoryId = categoryId,
                            OrganizationUnitId = organizationUnitId,
                            IsActive = true
                        };

                    _identityProvider.SetFor(categoryOrganizationUnit);
                    _categoryOrganizationUnitRepository.Add(categoryOrganizationUnit);
                    scope.Added<CategoryOrganizationUnit>(categoryOrganizationUnit.Id);
                }

                scope.Complete();
            }

            // Записи, которых не дожно быть.
            var linksToDelete = categoryUnits.Where(link => !organizationUnitIds.Contains(link.OrganizationUnitId));
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, CategoryOrganizationUnit>())
            {
                foreach (var categoryOrganizationUnit in linksToDelete)
                {
                    _categoryOrganizationUnitRepository.Delete(categoryOrganizationUnit);
                    scope.Deleted<CategoryOrganizationUnit>(categoryOrganizationUnit.Id);
                }

                scope.Complete();
            }
        }

        #region ImportCategoryLevel

        public void ImportCategoryLevel(IEnumerable<RubricServiceBusDto> categories, CategoryImportContext context)
        {
            foreach (var category in categories)
            {
                ImportCategory(category, context);
            }

            _categoryRepository.Save();
        }

        private void ImportCategory(RubricServiceBusDto dto, CategoryImportContext context)
        {
            var category = _finder.Find<Category>(x => x.Id == dto.Id)
                                  .SingleOrDefault() ??
                           new Category { Id = dto.Id };

            if (!dto.IsDeleted && !category.IsNew() && category.Level != dto.Level)
            {
                throw new ArgumentException(string.Format(BLResources.ChangingLevelOfRubricIsNotSupported, dto.Id), "dto");
            }

            if (category.IsNew())
            {
                CreateCategory(category, dto, context);
            }
            else
            {
                UpdateCategory(category, dto, context);
            }
        }

        private void UpdateCategory(Category category, RubricServiceBusDto dto, CategoryImportContext context)
        {
            if (dto.IsDeleted)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Category>())
                {
                    _categoryRepository.Delete(category);
                    scope.Deleted<Category>(category.Id).Complete();
                }
                
                if (category.ParentId.HasValue)
                {
                    context.PutAffectedCategory(category.Level - 1, category.ParentId.Value);
                }

                return;
            }

            CopySimpleFields(dto, category);

            if (dto.ParentId.HasValue)
            {
                // смело игнорим возможное отсутствие category.ParentId, так как он не может быть null при dto.ParentDgppId != null, а если такое вдруг случилось - упать не худшее решение среди возможных
                var newErmParentId = dto.ParentId.Value;
                if (category.ParentId.Value != newErmParentId)
                {
                    context.PutAffectedCategory(category.Level - 1, category.ParentId.Value);
                    category.ParentId = newErmParentId;
                }

                context.PutAffectedCategory(category.Level - 1, category.ParentId.Value);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Category>())
            {
                _categoryRepository.Update(category);
                scope.Updated<Category>(category.Id).Complete();
            }
        }

        private void CreateCategory(Category category, RubricServiceBusDto dto, CategoryImportContext context)
        {
            if (dto.IsDeleted)
            {
                return;
            }

            CopySimpleFields(dto, category);

            if (dto.ParentId.HasValue)
            {
                var ermParentId = dto.ParentId.Value;
                category.ParentId = ermParentId;
                context.PutAffectedCategory(category.Level - 1, ermParentId);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Category>())
            {
                _categoryRepository.Add(category);
                scope.Added<Category>(category.Id).Complete();
            }
        }

        private void CopySimpleFields(RubricServiceBusDto source, Category destination)
        {
            destination.Name = source.Name;
            destination.Level = source.Level;
            destination.Comment = source.Comment;
            destination.IsActive = !source.IsHidden;
            destination.IsDeleted = false; //Could have been deleted
        }

        #endregion

        #region Commons

        private IEnumerable<long> ResolveOrganizatuionUnitsDgppIds(IEnumerable<int> list, CategoryImportContext context)
        {
            var result = new List<long>();
            var unresolved = new List<int>();
            foreach (var dgppId in list)
            {
                long ermId;
                if (context.TryResolveOrganizationUnitDgppId(dgppId, out ermId))
                {
                    result.Add(ermId);
                }
                else
                {
                    unresolved.Add(dgppId);
                }
            }

            if (!unresolved.Any())
            {
                return result;
            }

            var resolvedFromDb = _finder.Find<OrganizationUnit>(unit => unit.DgppId.HasValue && unresolved.Contains(unit.DgppId.Value))
                                        .Select(unit => new { ErmId = unit.Id, DgppId = unit.DgppId.Value })
                                        .ToArray();

            if (resolvedFromDb.Length != unresolved.Count())
            {
                throw new ArgumentException(string.Format(BLResources.CannotFindOrgUnitWithDgppId,
                                                          unresolved.Except(resolvedFromDb.Select(i => i.DgppId)).First()));
            }

            foreach (var id in resolvedFromDb)
            {
                context.CacheOrganizationUnitDgppToErmId(id.DgppId, id.ErmId);
            }

            return result.Concat(resolvedFromDb.Select(i => i.ErmId));
        }

        #endregion

        [Obsolete]
        private static readonly IReadOnlyDictionary<long, IReadOnlyCollection<long>> SupportedCategoryOrganizationUnits =
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
    }
}