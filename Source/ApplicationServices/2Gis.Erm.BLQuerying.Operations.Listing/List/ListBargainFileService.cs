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
    public sealed class ListBargainFileService : ListEntityDtoServiceBase<BargainFile, ListBargainFileDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListBargainFileService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListBargainFileDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<BargainFile>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                {
                    x.Id,
                    x.FileId,
                    FileKindId = x.FileKind,
                    x.File.FileName,
                    x.CreatedOn,
                    x.BargainId,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListBargainFileDto
                {
                    Id = x.Id,
                    FileId = x.FileId,
                    FileKind = ((BargainFileKind)x.FileKindId).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    FileName = x.FileName,
                    CreatedOn = x.CreatedOn,
                    BargainId = x.BargainId,
                });
        }
    }
}