using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPositionService : ListEntityDtoServiceBase<Position, ListPositionDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPositionService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Position>();

            var supportedByExportFilter = querySettings.CreateForExtendedProperty<Position, bool>(
                "isSupportedByExport",
                isSupportedByExport => x => x.PositionCategory.IsSupportedByExport == isSupportedByExport);

            var compositeFilter = querySettings.CreateForExtendedProperty<Position, bool>(
                "composite",
                composite => x => x.IsComposite == composite);

            return query
                    .Filter(_filterHelper, supportedByExportFilter, compositeFilter)
                    .Select(x => new ListPositionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PlatformName = x.Platform.Name,
                        IsComposite = x.IsComposite,
                        CategoryName = x.PositionCategory.Name,
                        ExportCode = x.ExportCode,
                        RestrictChildPositionPlatforms = x.RestrictChildPositionPlatforms,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                    .QuerySettings(_filterHelper, querySettings);
        }
    }
}
