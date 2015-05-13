using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPositionService : ListEntityDtoServiceBase<Position, ListPositionDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListPositionService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Position>();

            return query
                    .Select(x => new ListPositionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PlatformName = x.Platform.Name,
                        IsComposite = x.IsComposite,
                        CategoryName = x.PositionCategory.Name,
                        ExportCode = x.ExportCode,
                        RestrictChildPositionPlatforms = x.RestrictChildPositionPlatforms,
                        IsSupportedByExport = x.PositionCategory.IsSupportedByExport,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        PositionsGroup = x.PositionsGroup.ToStringLocalizedExpression()
                    })
                    .QuerySettings(_filterHelper, querySettings);
        }
    }
}
