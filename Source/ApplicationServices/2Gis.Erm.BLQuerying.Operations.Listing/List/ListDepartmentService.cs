using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListDepartmentService : ListEntityDtoServiceBase<Department, ListDepartmentDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListDepartmentService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListDepartmentDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Department>();

            var excludeIdFilter = querySettings.CreateForExtendedProperty<Department, long>(
                "excludeId",
                excludeId => x => x.Id != excludeId);

            return query
                .Filter(_filterHelper, excludeIdFilter)
                .Select(x => new ListDepartmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    ParentName = x.Parent.Name,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}