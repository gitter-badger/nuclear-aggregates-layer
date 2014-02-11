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
    public class ListBargainFileService : ListEntityDtoServiceBase<BargainFile, ListBargainFileDto>
    {
        public ListBargainFileService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListBargainFileDto> GetListData(IQueryable<BargainFile> query, QuerySettings querySettings, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new { x.Id, x.FileId, FileKindId = x.FileKind, x.File.FileName, x.CreatedOn })
                .AsEnumerable()
                .Select(x =>
                        new ListBargainFileDto
                            {
                                Id = x.Id,
                                FileId = x.FileId,
                                FileKind = ((BargainFileKind)x.FileKindId).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                FileName = x.FileName,
                                CreatedOn = x.CreatedOn
                            });
        }
    }
}