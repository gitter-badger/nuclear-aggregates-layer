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
    public sealed class ListThemeService : ListEntityDtoServiceBase<Theme, ListThemeDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListThemeService(
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListThemeDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Theme>();
            var themeOrganizationUnits = _finder.FindAll<ThemeOrganizationUnit>();

            // dto от тематик
            var dtosQuery = query
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.BeginDistribution,
                    x.EndDistribution,
                    x.ThemeTemplate.TemplateCode,
                    x.Description,
                    x.IsActive,
                    x.IsDeleted,
                    OrganizationUnitName = (string)null,
                });

            // dto от объекта связи тематики и отделения организации
            var dtosQuery2 = themeOrganizationUnits.Join(dtosQuery, x => x.ThemeId, x => x.Id, (x, y) => new
            {
                y.Id,
                y.Name,
                y.BeginDistribution,
                y.EndDistribution,
                y.TemplateCode,
                y.Description,
                y.IsActive,
                y.IsDeleted,
                OrganizationUnitName = x.OrganizationUnit.Name
            });

            var allDtosQuery = dtosQuery.Union(dtosQuery2);

            // distinct без учёта OrganizationUnitName
            var data = allDtosQuery
                        .Select(x => new ListThemeDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            BeginDistribution = x.BeginDistribution,
                            EndDistribution = x.EndDistribution,
                            TemplateCodeEnum = (ThemeTemplateCode)x.TemplateCode,
                            Description = x.Description,
                            IsActive = x.IsActive,
                            IsDeleted = x.IsDeleted,
                            TemplateCode = null,
                        })
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings, out count)
                        .Select(x =>
                        {
                            x.TemplateCode = x.TemplateCodeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                            return x;
                        });

            return data;
        }
    }
}