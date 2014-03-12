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
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListThemeTemplateService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListThemeTemplateDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<ThemeTemplate>();

            return query
                .Where(Specs.Find.ActiveAndNotDeleted<ThemeTemplate>())
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                {
                    x.Id,
                    x.TemplateCode,
                    x.File.FileName
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListThemeTemplateDto
                {
                    Id = x.Id,
                    TemplateCode = ((ThemeTemplateCode)x.TemplateCode).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    FileName = x.FileName
                });
        }
    }
}