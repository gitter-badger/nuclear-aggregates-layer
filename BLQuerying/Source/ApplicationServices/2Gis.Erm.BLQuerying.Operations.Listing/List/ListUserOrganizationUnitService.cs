using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserOrganizationUnitService : ListEntityDtoServiceBase<UserOrganizationUnit, ListUserOrganizationUnitDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListUserOrganizationUnitService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<UserOrganizationUnit>();

            var data = query
                .Select(x => new ListUserOrganizationUnitDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnitDto.Name,
                    UserName = x.User.DisplayName,
                    UserDepartmentName = x.User.Department.Name,
                    UserRoleName = x.User.UserRoles.Select(y => y.Role.Name),
                    UserIsActive = x.User.IsActive,
                    UserIsDeleted = x.User.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}