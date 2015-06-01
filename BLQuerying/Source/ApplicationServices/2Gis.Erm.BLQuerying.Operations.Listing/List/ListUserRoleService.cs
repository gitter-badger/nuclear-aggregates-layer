using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserRoleService : ListEntityDtoServiceBase<UserRole, ListUserRoleDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListUserRoleService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<UserRole>();

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