using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAssociatedPositionService : ListEntityDtoServiceBase<AssociatedPosition, ListAssociatedPositionDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListAssociatedPositionService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<AssociatedPosition>();

            return query
                .Select(x => new ListAssociatedPositionDto
                    {
                        Id = x.Id,
                        PositionId = x.PositionId,
                        PositionName = x.Position.Name,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        AssociatedPositionsGroupId = x.AssociatedPositionsGroupId,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}