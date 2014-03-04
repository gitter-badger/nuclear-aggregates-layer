using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAssociatedPositionService : ListEntityDtoServiceBase<AssociatedPosition, ListAssociatedPositionDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAssociatedPositionService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAssociatedPositionDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AssociatedPosition>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListAssociatedPositionDto
                    {
                        Id = x.Id,
                        PositionId = x.PositionId,
                        PositionName = x.Position.Name,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        AssociatedPositionsGroupId = x.AssociatedPositionsGroupId,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}