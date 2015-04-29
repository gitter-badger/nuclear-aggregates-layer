using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListThemeOrganizationUnitService : ListEntityDtoServiceBase<ThemeOrganizationUnit, ListThemeOrganizationUnitDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListThemeOrganizationUnitService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<ThemeOrganizationUnit>();

            return query
                .Select(x => new ListThemeOrganizationUnitDto
                    {
                        Id = x.Id,
                        OrganizationUnitId = x.OrganizationUnitId,
                        OrganizationUnitName = x.OrganizationUnit.Name,
                        ThemeId = x.ThemeId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}