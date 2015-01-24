using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public class CategoryServiceTest : IIntegrationTest
    {
        private const long NewId = 248015866241744904;
        private readonly ICategoryService _categoryService;

        public CategoryServiceTest(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ITestResult Execute()
        {
            var newCategoryGroup = new CategoryGroup { Name = "Test", GroupRate = 0.007m, Id = NewId };
            _categoryService.CreateOrUpdate(newCategoryGroup);

            _categoryService.Delete(newCategoryGroup);


            var dtos = new[]
                {
                    new RubricServiceBusDto
                        {
                            Comment = "Test",
                            Id = NewId,
                            Level = 1,
                            Name = "Test",
                            OrganizationUnitsDgppIds = new[]
                                {
                                    1
                                }
                        }
                };

            var context = new CategoryImportContext();

            _categoryService.ImportCategoryLevel(dtos, context);

            _categoryService.ImportOrganizationUnits(dtos, context);

            _categoryService.FixAffectedCategories(1, context);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}