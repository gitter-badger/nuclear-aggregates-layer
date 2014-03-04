using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPositionChildrenService : ListEntityDtoServiceBase<PositionChildren, ListPositionChildrenDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPositionChildrenService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPositionChildrenDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<PositionChildren>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListPositionChildrenDto
                    {
                        Id = x.Id,
                        Name = x.ChildPosition.Name,
                        PlatformName = x.ChildPosition.Platform.Name,
                        CategoryName = x.ChildPosition.PositionCategory.Name,
                        MasterPositionId = x.MasterPositionId,
                        ExportCode = x.ChildPosition.ExportCode,
                        ChildPositionId = x.ChildPositionId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}