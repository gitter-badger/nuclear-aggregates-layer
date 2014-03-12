using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListThemeCategoryService : ListEntityDtoServiceBase<ThemeCategory, ListThemeCategoryDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListThemeCategoryService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListThemeCategoryDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<ThemeCategory>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListThemeCategoryDto
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.Name,
                        IsDeleted = x.IsDeleted,
                        ThemeId = x.ThemeId,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}