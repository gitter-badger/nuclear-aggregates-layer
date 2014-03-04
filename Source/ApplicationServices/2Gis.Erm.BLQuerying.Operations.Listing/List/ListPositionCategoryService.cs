using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPositionCategoryService : ListEntityDtoServiceBase<PositionCategory, ListPositionCategoryDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPositionCategoryService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPositionCategoryDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<PositionCategory>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListPositionCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ExportCode = x.ExportCode,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}