using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPositionChildrenService : ListEntityDtoServiceBase<PositionChildren, ListPositionChildrenDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListPositionChildrenService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<PositionChildren>();

            return query
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
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}