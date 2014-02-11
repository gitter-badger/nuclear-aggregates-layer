using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListThemeTemplateService : ListEntityDtoServiceBase<ThemeTemplate, ListThemeTemplateDto>
    {
        public ListThemeTemplateService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListThemeTemplateDto> GetListData(IQueryable<ThemeTemplate> query, QuerySettings querySettings, out int count)
        {
            return query
                .Where(Specs.Find.ActiveAndNotDeleted<ThemeTemplate>())
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.TemplateCode,
                        x.File.FileName
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListThemeTemplateDto
                            {
                                Id = x.Id,
                                TemplateCode = ((ThemeTemplateCode)x.TemplateCode).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                FileName = x.FileName
                            });
        }
    }
}