using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Rubrics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public interface ICategoryService : ISimplifiedModelConsumer
    {
        void ImportCategoryLevel(IEnumerable<RubricServiceBusDto> categories, CategoryImportContext context);
        void ImportOrganizationUnits(IEnumerable<RubricServiceBusDto> categories, CategoryImportContext context);
        void FixAffectedCategories(int level, CategoryImportContext context);
        void SetCategoryGroupMembership(long organizationUnitId, IEnumerable<CategoryGroupMembershipDto> membership);
        void Delete(CategoryGroup categoryGroup);
        void CreateOrUpdate(CategoryGroup entity);
        string GetCategoryName(long categoryId);
        IReadOnlyDictionary<long, int> GetCategoryLevels(IEnumerable<long> categoryIds);
        IDictionary<long, IEnumerable<long>> GetFirmAddressesCategories(long destOrganizationUnitId, IEnumerable<long> firmAddressIds);
        IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetFirmCategories(IEnumerable<long> firmCategoryIds);
        IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetAdditionalCategories(IEnumerable<long> firmCategoryIds, long orderPositionId);
        IEnumerable<long> GetCategoriesSupportedBySalesModelInOrganizationUnit(SalesModel salesModel, long organizationUnitId);

        IDictionary<long, string> PickCategoriesUnsupportedBySalesModelInOrganizationUnit(SalesModel salesModel,
                                                                                          long destOrganizationUnitId,
                                                                                          IEnumerable<long> categoryIds);
    }
}
