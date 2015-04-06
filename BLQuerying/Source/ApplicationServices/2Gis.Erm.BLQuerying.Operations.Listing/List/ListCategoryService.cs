using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCategoryService : ListEntityDtoServiceBase<Category, ListCategoryDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListCategoryService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Category>();

            // Фильтр рубрик, которые можно добавить в тематику (рубрики есть во всех подразделениях тематики)
            if (querySettings.ParentEntityName == EntityName.Theme)
            {
                var themeId = querySettings.ParentEntityId;

                var unitCount = _finder.FindAll<OrganizationUnit>()
                                  .Count(unit => unit.ThemeOrganizationUnits.Any(link => link.IsActive
                                                                                         && !link.IsDeleted
                                                                                         && link.Theme.Id == themeId));

                query = _finder.FindAll<OrganizationUnit>()

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
                       .Where(group => @group.Count() == unitCount)
                       .Select(group => @group.Key);
            }

            var firmIdFilter = querySettings.CreateForExtendedProperty<Category, long>(
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
            var firmAddressIdFilter = querySettings.CreateForExtendedProperty<Category, long>(
                "firmAddressId",
                firmAddressId => x => x.CategoryFirmAddresses.Any(y => !y.IsDeleted && y.IsActive && y.FirmAddressId == firmAddressId) ||
                                                       (from childCategory2 in x.ChildCategories
                                                        where childCategory2.IsActive && !childCategory2.IsDeleted
                                                        from childCategory1 in childCategory2.ChildCategories
                                                        where childCategory1.IsActive && !childCategory1.IsDeleted
                                                        from categoryFirmAddress in childCategory1.CategoryFirmAddresses
                                                        select categoryFirmAddress).Any(y => !y.IsDeleted && y.IsActive && y.FirmAddressId == firmAddressId));

            var organizationUnitIdFilter = querySettings.CreateForExtendedProperty<Category, long>(
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

            var salesModelFilter =
                querySettings.CreateForExtendedProperty<Category, int>("salesModel",
                                                                              salesModel =>
                                                                              {
                                                                                  long organizationUnitId;
                                                                                  int positionsGroup;
                                                                                  if (!querySettings.TryGetExtendedProperty("OrganizationUnitId",
                                                                                      out organizationUnitId))
                                                                                  {
                                                                                      return x => false;
                                                                                  }

                                                                                  if (querySettings.TryGetExtendedProperty("PositionsGroup", out positionsGroup) &&
                                                                                      (PositionsGroup)positionsGroup == PositionsGroup.Media)
                                                                                  {
                                                                                      return x => true;
                                                                                  }

                                                                                  var specification = CategorySpecs.Categories.Find.ActiveCategoryForSalesModelInOrganizationUnit((SalesModel)salesModel, organizationUnitId);
                                                                                  return specification.Predicate;
                                                                              });

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper
                , firmIdFilter
                , firmAddressIdFilter
                , organizationUnitIdFilter
                , salesModelFilter)
                .Select(x => new ListCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    ParentName = x.ParentCategory != null ? x.ParentCategory.Name : null,
                    Level = x.Level,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}