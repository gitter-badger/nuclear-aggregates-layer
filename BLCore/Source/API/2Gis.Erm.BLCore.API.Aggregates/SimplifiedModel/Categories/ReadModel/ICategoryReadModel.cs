using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public interface ICategoryReadModel : ISimplifiedModelConsumerReadModel
    {
        string GetCategoryName(long categoryId);
        IReadOnlyDictionary<long, int> GetCategoryLevels(IEnumerable<long> categoryIds);
        IDictionary<long, IEnumerable<long>> GetFirmAddressesCategories(long destOrganizationUnitId, IEnumerable<long> firmAddressIds);
        IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetFirmCategories(IEnumerable<long> firmCategoryIds, SalesModel salesModel, long organizationUnitId);
        IEnumerable<LinkingObjectsSchemaDto.CategoryDto> GetAdditionalCategories(IEnumerable<long> firmCategoryIds, long orderPositionId, SalesModel salesModel, long organizationUnitId);
        IDictionary<long, string> PickCategoriesUnsupportedBySalesModelInOrganizationUnit(SalesModel salesModel,
                                                                                          long destOrganizationUnitId,
                                                                                          IEnumerable<long> categoryIds);
    }
}