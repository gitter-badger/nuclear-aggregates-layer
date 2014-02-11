using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListPositionService : ListEntityDtoServiceBase<Position, ListPositionDto>
    {
        public ListPositionService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListPositionDto> GetListData(IQueryable<Position> query, QuerySettings querySettings, out int count)
        {
            var supportedByExportFilter = querySettings.CreateForExtendedProperty<Position, bool>(
                "isSupportedByExport",
                isSupportedByExport => x => x.PositionCategory.IsSupportedByExport == isSupportedByExport);

            var compositeFilter = querySettings.CreateForExtendedProperty<Position, bool>(
                "composite",
                composite => x => x.IsComposite == composite);

            return
                query
                     .ApplyFilter(supportedByExportFilter)
                     .ApplyFilter(compositeFilter)
                     .ApplyQuerySettings(querySettings, out count)
                     .Select(x =>
                             new ListPositionDto
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     PlatformName = x.Platform.Name,
                                     IsComposite = x.IsComposite,
                                     CategoryName = x.PositionCategory.Name,
                                     ExportCode = x.ExportCode,
                                     RestrictChildPositionPlatforms = x.RestrictChildPositionPlatforms
                                 });
        }
    }
}
