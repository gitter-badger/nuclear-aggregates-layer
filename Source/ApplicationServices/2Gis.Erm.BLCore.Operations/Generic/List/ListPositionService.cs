using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
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

        protected override IEnumerable<ListPositionDto> GetListData(IQueryable<Position> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var supportedByExportFilter = filterManager.CreateForExtendedProperty<Position, bool>(
                "isSupportedByExport",
                isSupportedByExport => x => x.PositionCategory.IsSupportedByExport == isSupportedByExport);

            return
                query.ApplyFilter(supportedByExportFilter)
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
