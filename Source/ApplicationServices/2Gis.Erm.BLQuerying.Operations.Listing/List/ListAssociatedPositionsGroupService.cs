using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAssociatedPositionsGroupService : ListEntityDtoServiceBase<AssociatedPositionsGroup, ListAssociatedPositionsGroupDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAssociatedPositionsGroupService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAssociatedPositionsGroupDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AssociatedPositionsGroup>();

            return query
                .Select(x => new ListAssociatedPositionsGroupDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PricePositionId = x.PricePositionId,
                        PricePositionName = x.PricePosition.Position.Name,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}