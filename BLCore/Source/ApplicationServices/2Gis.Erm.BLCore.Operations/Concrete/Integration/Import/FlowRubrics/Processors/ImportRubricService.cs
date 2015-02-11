using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Category;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowRubrics.Processors
{
    public class ImportRubricService : IImportRubricService
    {
        private readonly ICategoryService _categoryService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICategoryReadModel _categoryReadModel;

        public ImportRubricService(ICategoryService categoryService, IOperationScopeFactory scopeFactory, ICategoryReadModel categoryReadModel)
        {
            _categoryService = categoryService;
            _scopeFactory = scopeFactory;
            _categoryReadModel = categoryReadModel;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var rubricServiceBusDtos = dtos.Cast<RubricServiceBusDto>().ToArray();

            // для удаленных рубрик тащим уровень из базы
            var deletedDtos = rubricServiceBusDtos.Where(x => x.IsDeleted).ToArray();
            var categoryIdToLevelMap = _categoryReadModel.GetCategoryLevels(deletedDtos.Select(x => x.Id)); 
            foreach (var dto in deletedDtos)
            {
                int level;
                if (categoryIdToLevelMap.TryGetValue(dto.Id, out level))
                {
                    dto.Level = level;
                }
            }

            using (var scope = _scopeFactory.CreateNonCoupled<ImportRubricIdentity>())
            {
                var context = new CategoryImportContext();
                foreach (var dto in rubricServiceBusDtos)
                {
                    Validate(dto);
                }

                ImportCategories(rubricServiceBusDtos, context);
                ImportOrganizationUnitLinks(rubricServiceBusDtos, context);

                scope.Complete();
            }
        }

        private void ImportCategories(IEnumerable<RubricServiceBusDto> categories, CategoryImportContext context)
        {
            // рубрики импортируются слоями, начиная с самого верхнего, 
            // так как дочерние рубрики могут ссылаться на те, которые ранее не существовали и пришли только в этом пакете.
            var categoriesByLevel = categories.GroupBy(dto => dto.Level).OrderBy(group => group.Key);
            
            // todo: продумать кейс с удалением рубрики второго уровня (что должно происходить, если есть подчинённые рубрики третьего уровня?)
            foreach (var level in categoriesByLevel)
            {
                _categoryService.ImportCategoryLevel(level, context);
            }
        }

        private void ImportOrganizationUnitLinks(IEnumerable<RubricServiceBusDto> categories, CategoryImportContext context)
        {
            // реально подразделения указываются только для рубрик третьего уровня,
            // а для всех родительских они вычисляются объединением всех дочерних.
            _categoryService.ImportOrganizationUnits(categories.Where(cat => cat.Level == 3), context);

            foreach (var level in new[] { 2, 1 })
            {
                _categoryService.FixAffectedCategories(level, context);
            }
        }

        private void Validate(RubricServiceBusDto category)
        {
            // Don't need to validate deleted categories
            if (category.IsDeleted)
            {
                return;
            }

            if (category.Level < 1 || category.Level > 3)
            {
                throw CreateBusinessException("Ошибка при импорте рубрики с Id={0}: уровень рубрики {1} находится за пределами допустимого диапазона (1, 2, 3).",
                                              category.Id,
                                              category.Level);
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw CreateBusinessException("Ошибка при импорте рубрики с Id={0}: название рубрики не должно быть пустым.", category.Id);
            }

            if (category.Level > 1 && category.ParentId == null)
            {
                throw CreateBusinessException("Ошибка при импорте рубрики с Id={0}: только рубрики первого уровня могут иметь ParentId=null",
                                              category.Id);
            }
        }

        private BusinessLogicException CreateBusinessException(string message, params object[] parameters)
        {
            return new BusinessLogicException(string.Format(message, parameters));
        }
    }
}