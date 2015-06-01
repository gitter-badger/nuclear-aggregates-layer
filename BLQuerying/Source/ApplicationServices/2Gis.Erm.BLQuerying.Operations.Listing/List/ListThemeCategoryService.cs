using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListThemeCategoryService : ListEntityDtoServiceBase<ThemeCategory, ListThemeCategoryDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListThemeCategoryService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<ThemeCategory>();

            return query
                .Select(x => new ListThemeCategoryDto
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.Name,
                        IsDeleted = x.IsDeleted,
                        ThemeId = x.ThemeId,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}