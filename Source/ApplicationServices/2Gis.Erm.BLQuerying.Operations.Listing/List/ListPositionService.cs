using System.Collections.Generic;
using System.Linq;

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

        public ListPositionService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPositionDto> List(QuerySettings querySettings, out int count)
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
                    .DefaultFilter(_filterHelper, querySettings)
                    .Select(x => new ListPositionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PlatformName = x.Platform.Name,
                        IsComposite = x.IsComposite,
                        CategoryName = x.PositionCategory.Name,
                        ExportCode = x.ExportCode,
                        RestrictChildPositionPlatforms = x.RestrictChildPositionPlatforms
                    })
                    .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}
