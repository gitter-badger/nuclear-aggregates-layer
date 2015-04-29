using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListThemeService : ListEntityDtoServiceBase<Theme, ListThemeDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListThemeService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<Theme>();
            var themeOrganizationUnits = _finder.For<ThemeOrganizationUnit>();

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
                Description = x.Description,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                TemplateCode = ((ThemeTemplateCode)x.TemplateCode).ToStringLocalizedExpression(),
            })
            .Distinct()
            .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}