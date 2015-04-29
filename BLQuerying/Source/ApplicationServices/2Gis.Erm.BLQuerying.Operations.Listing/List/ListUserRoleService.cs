using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserRoleService : ListEntityDtoServiceBase<UserRole, ListUserRoleDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListUserRoleService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<UserRole>();

            var data = query
            .Select(x => new ListUserRoleDto
            {
                Id = x.Id,
                UserId = x.UserId,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
            })
            .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}