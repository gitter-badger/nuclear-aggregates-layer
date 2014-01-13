using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListOrderFileService : ListEntityDtoServiceBase<OrderFile, ListOrderFileDto>
    {
        public ListOrderFileService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListOrderFileDto> GetListData(IQueryable<OrderFile> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.FileId,
                        FileKind = (OrderFileKind)x.FileKind,
                        x.File.FileName
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListOrderFileDto
                            {
                                Id = x.Id, 
                                FileId = x.FileId,
                                FileKind = x.FileKind.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo), 
                                FileName = x.FileName
                            })
                .OrderBy(x => x.FileKind);
        }
    }
}