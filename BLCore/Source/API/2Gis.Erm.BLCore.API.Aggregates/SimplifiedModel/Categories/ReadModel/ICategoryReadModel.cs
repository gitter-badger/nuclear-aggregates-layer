using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public interface ICategoryReadModel : ISimplifiedModelConsumerReadModel
    {
        string GetCategoryName(long categoryId);
        IReadOnlyDictionary<long, int> GetCategoryLevels(IEnumerable<long> categoryIds);
        IDictionary<long, IEnumerable<LinkingObjectsSchemaCategoryDto>> GetFirmAddressesCategories(long destOrganizationUnitId, IEnumerable<long> firmAddressIds);
        IEnumerable<LinkingObjectsSchemaCategoryDto> GetFirmCategories(IEnumerable<long> firmCategoryIds, SalesModel salesModel, long organizationUnitId);
        IEnumerable<CategoryAsLinkingObjectDto> GetSalesIntoCategories(long orderPositionId);
        IDictionary<long, string> PickCategoriesUnsupportedBySalesModelInOrganizationUnit(SalesModel salesModel, long destOrganizationUnitId, IEnumerable<long> categoryIds);
        IEnumerable<CategoryGroupDto> GetCategoryGroups();
        IEnumerable<CategoryGroupMembershipDto> GetCategoryGroupMembership(long organizationUnitId);
        IEnumerable<CategoryOrganizationUnitDto> GetCategoryOrganizationUnits(IEnumerable<long> ids);
    }
}