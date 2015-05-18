using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Categories
{
    public sealed class CategoryService : ICategoryService
    {
        private const long DefaultCategoryGroupId = 3;

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

        public void Delete(CategoryGroup categoryGroup)
        {
            var linkedCategories = _finder.Find<CategoryOrganizationUnit>(x => x.CategoryGroupId == categoryGroup.Id).ToArray();

            if (categoryGroup.Id == DefaultCategoryGroupId)
            {
                throw new InvalidOperationException(BLResources.CanNotDeleteDefaultCategoryGroup);
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var category in linkedCategories)
                {
                    category.CategoryGroupId = DefaultCategoryGroupId;
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
                            CategoryGroupId = DefaultCategoryGroupId,
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
    }
}