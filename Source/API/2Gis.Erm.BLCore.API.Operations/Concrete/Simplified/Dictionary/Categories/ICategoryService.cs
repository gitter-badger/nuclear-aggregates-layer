using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public interface ICategoryService : ISimplifiedModelConsumer
    {
        void ImportCategoryLevel(IEnumerable<CategoryImportServiceBusDto> categories, CategoryImportContext context);
        void ImportOrganizationUnits(IEnumerable<CategoryImportServiceBusDto> categories, CategoryImportContext context);
        void FixAffectedCategories(int level, CategoryImportContext context);
        void SetCategoryGroupMembership(long organizationUnitId, IEnumerable<CategoryGroupMembershipDto> membership);
        void Delete(CategoryGroup categoryGroup);
        void CreateOrUpdate(CategoryGroup entity);
        bool IsCategoryLinkedWithOrgUnit(long categoryId, long organizationUnitId);
        string GetCategoryName(long categoryId);
    }
}
