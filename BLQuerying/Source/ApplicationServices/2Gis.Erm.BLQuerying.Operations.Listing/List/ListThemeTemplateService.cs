using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListThemeTemplateService : ListEntityDtoServiceBase<ThemeTemplate, ListThemeTemplateDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListThemeTemplateService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<ThemeTemplate>();

            return query
                .Where(Specs.Find.ActiveAndNotDeleted<ThemeTemplate>())
                .Select(x => new ListThemeTemplateDto
                {
                    Id = x.Id,
                    FileName = x.File.FileName,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    TemplateCode = ((ThemeTemplateCode)x.TemplateCode).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}