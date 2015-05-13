using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAssociatedPositionsGroupService : ListEntityDtoServiceBase<AssociatedPositionsGroup, ListAssociatedPositionsGroupDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListAssociatedPositionsGroupService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<AssociatedPositionsGroup>();

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
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}