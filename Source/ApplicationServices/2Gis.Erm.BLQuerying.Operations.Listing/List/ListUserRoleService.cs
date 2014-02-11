using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserRoleService : ListEntityDtoServiceBase<UserRole, ListUserRoleDto>
    {
        public ListUserRoleService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListUserRoleDto> GetListData(IQueryable<UserRole> query, QuerySettings querySettings, out int count)
        {
            var data = query
            .Select(x => new
        {
                // filters
                x.UserId,

                x.Id,
                x.RoleId,
                RoleName = x.Role.Name,
            })
            .ApplyQuerySettings(querySettings, out count)
            .AsEnumerable()
            .Select(x => new ListUserRoleDto
            {
                Id = x.Id,
                RoleId = x.RoleId,
                RoleName = x.RoleName,
            });

            return data;
        }
    }
}