using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using System.Collections.Generic;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{

    public sealed class ListUserOrganizationUnitService : ListEntityDtoServiceBase<UserOrganizationUnit, ListUserOrganizationUnitDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListUserOrganizationUnitService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListUserOrganizationUnitDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<UserOrganizationUnit>();

            var data = query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListUserOrganizationUnitDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnitDto.Name,
                    UserName = x.User.DisplayName,
                    UserDepartmentName = x.User.Department.Name,
                    UserRoleName = x.User.UserRoles.Select(y => y.Role.Name)
                })
                .QuerySettings(_filterHelper, querySettings, out count);

            return data;
        }
    }
}