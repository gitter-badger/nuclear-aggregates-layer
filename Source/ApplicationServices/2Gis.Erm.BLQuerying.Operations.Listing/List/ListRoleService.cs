using System.Collections.Generic;
using System.Linq;

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

        public ListRoleService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListRoleDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Role>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListRoleDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}