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
    public class ListDepartmentService : ListEntityDtoServiceBase<Department, ListDepartmentDto>
    {
        public ListDepartmentService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListDepartmentDto> GetListData(IQueryable<Department> query, QuerySettings querySettings, out int count)
        {
            var excludeIdFilter = querySettings.CreateForExtendedProperty<Department, long>(
                "excludeId",
                excludeId => x => x.Id != excludeId);

            return query
                .ApplyFilter(excludeIdFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new ListDepartmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    ParentName = x.Parent.Name,
                });
        }
    }
}