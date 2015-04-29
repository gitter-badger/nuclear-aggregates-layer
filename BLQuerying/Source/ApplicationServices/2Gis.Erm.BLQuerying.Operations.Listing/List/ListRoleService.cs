using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListRoleService : ListEntityDtoServiceBase<Role, ListRoleDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListRoleService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<Role>();

            return query
                .Select(x => new ListRoleDto
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}