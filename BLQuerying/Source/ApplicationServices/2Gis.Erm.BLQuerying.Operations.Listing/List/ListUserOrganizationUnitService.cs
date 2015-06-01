using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{

    public sealed class ListUserOrganizationUnitService : ListEntityDtoServiceBase<UserOrganizationUnit, ListUserOrganizationUnitDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListUserOrganizationUnitService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<UserOrganizationUnit>();

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