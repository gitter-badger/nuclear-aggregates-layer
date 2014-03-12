using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListProjectService : ListEntityDtoServiceBase<Project, ListProjectDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListProjectService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListProjectDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Project>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListProjectDto
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        OrganizationUnitId = x.OrganizationUnitId,
                        OrganizationUnitName = x.OrganizationUnit.Name,
                        IsActive = x.IsActive,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}