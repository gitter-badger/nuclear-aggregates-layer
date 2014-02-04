using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListCategoryService : ListEntityDtoServiceBase<Category, ListCategoryDto>
    {
        public ListCategoryService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListCategoryDto> GetListData(IQueryable<Category> query,
                                                                    QuerySettings querySettings,
                                                                    ListFilterManager filterManager,
                                                                    out int count)
        {
            // Фильтр рубрик, которые можно добавить в тематику (рубрики есть во всех подразделениях тематики)
            if (filterManager.ParentEntityName == EntityName.Theme)
            {
                var themeId = filterManager.ParentEntityId;
                var finder = FinderBaseProvider.GetFinderBase(EntityName.Theme);
                var unitCount = finder.FindAll<OrganizationUnit>()
                                  .Count(unit => unit.ThemeOrganizationUnits.Any(link => link.IsActive
                                                                                         && !link.IsDeleted
                                                                                         && link.Theme.Id == themeId));

                query = finder.FindAll<OrganizationUnit>()

                              // Только те подразделения, в которых есть рубрика
                              .Where(unit => unit.ThemeOrganizationUnits.Any(link => link.IsActive
                                                                                     && !link.IsDeleted
                                                                                     && link.Theme.Id == themeId))

                              // Все не удаленные объекты связи между не удаленными рубриками и подразделениями, в которых есть рубрика третьего уровня
                              .SelectMany(unit => unit.CategoryOrganizationUnits)
                              .Where(link => link.IsActive && !link.IsDeleted && link.Category.IsActive && !link.Category.IsDeleted && link.Category.Level == 3)

                              // Считаем, в скольки подразделениях присутсвует рубрика
                              .GroupBy(link => link.Category)

                              // Рубрика подходит, если присутсвует во всех подразделениях
                              .Where(group => group.Count() == unitCount)
                              .Select(group => group.Key);
            }

            var firmIdFilter = filterManager.CreateForExtendedProperty<Category, long>(
                "firmId",
                firmId => x => x.CategoryFirmAddresses
                                .Any(y => !y.IsDeleted && y.IsActive &&
                                          !y.FirmAddress.IsDeleted && y.FirmAddress.IsActive &&
                                          y.FirmAddress.FirmId == firmId) ||
                               (from childCategory2 in x.ChildCategories
                                where childCategory2.IsActive && !childCategory2.IsDeleted
                                from childCategory1 in childCategory2.ChildCategories
                                where childCategory1.IsActive && !childCategory1.IsDeleted
                                from categoryFirmAddress in childCategory1.CategoryFirmAddresses
                                select categoryFirmAddress).Any(y => !y.IsDeleted && y.IsActive && y.FirmAddress.FirmId == firmId));

            // comment for expression debugging purposes
            /*            
                    x => x.CategoryFirmAddresses.Any(y => !y.IsDeleted && y.IsActive && !y.FirmAddress.IsDeleted && y.FirmAddress.IsActive &&y.FirmAddress.FirmId == firmId) || 
                        (x.ChildCategories.Where(z => z.IsActive && !z.IsDeleted)
                                .SelectMany(child => child.ChildCategories)
                                .Where(child => child.IsActive && !child.IsDeleted)
                                .SelectMany(child => child.CategoryFirmAddresses)
                                .Any(link => !link.IsDeleted && link.IsActive && link.FirmAddress.FirmId == firmId)));
            */
            var firmAddressIdFilter = filterManager.CreateForExtendedProperty<Category, long>(
                "firmAddressId",
                firmAddressId => x => x.CategoryFirmAddresses.Any(y => !y.IsDeleted && y.IsActive && y.FirmAddressId == firmAddressId) ||
                                                       (from childCategory2 in x.ChildCategories
                                                        where childCategory2.IsActive && !childCategory2.IsDeleted
                                                        from childCategory1 in childCategory2.ChildCategories
                                                        where childCategory1.IsActive && !childCategory1.IsDeleted
                                                        from categoryFirmAddress in childCategory1.CategoryFirmAddresses
                                                        select categoryFirmAddress).Any(y => !y.IsDeleted && y.IsActive && y.FirmAddressId == firmAddressId));

            var isActiveFilter = filterManager.CreateForExtendedProperty<Category, bool>(
                "IsActive", isActive => item => item.IsActive == isActive);

            var organizationUnitIdFilter = filterManager.CreateForExtendedProperty<Category, long>(
                "OrganizationUnitId",
                organizationUnitId => x => x.CategoryOrganizationUnits
                                            .Any(y => y.IsActive && !y.IsDeleted && y.OrganizationUnitId == organizationUnitId) ||
                                                                 x.ChildCategories
                                                                        .Any(childCategory2 => childCategory2.IsActive &&
                                                                                               !childCategory2.IsDeleted &&
                                                                                               childCategory2.ChildCategories
                                                                                                                    .Any(childCategory1 => childCategory1.IsActive &&
                                                                                                                                           !childCategory1.IsDeleted &&
                                                                                                                                           childCategory1.CategoryOrganizationUnits
                                                                                                                                                                .Any(y => y.IsActive &&
                                                                                                                                                                            !y.IsDeleted &&
                                                                                                                                                                            y.OrganizationUnitId == organizationUnitId))));
            return query
                .Where(x => !x.IsDeleted)
                .ApplyFilter(firmIdFilter)
                .ApplyFilter(firmAddressIdFilter)
                .ApplyFilter(isActiveFilter)
                .ApplyFilter(organizationUnitIdFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new ListCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    ParentName = x.ParentCategory != null ? x.ParentCategory.Name : null,
                    Level = x.Level
                });
        }
    }
}