using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderFileService : ListEntityDtoServiceBase<OrderFile, ListOrderFileDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListOrderFileService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListOrderFileDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<OrderFile>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                {
                    x.Id,
                    x.CreatedOn,
                    x.FileId,
                    FileKind = (OrderFileKind)x.FileKind,
                    x.File.FileName,
                    x.OrderId,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListOrderFileDto
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    FileId = x.FileId,
                    FileKind = x.FileKind.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo), 
                    FileName = x.FileName,
                    OrderId = x.OrderId,
                })
                // TODO: почему форсится OrderBy
                .OrderBy(x => x.FileKind);
        }
    }
}