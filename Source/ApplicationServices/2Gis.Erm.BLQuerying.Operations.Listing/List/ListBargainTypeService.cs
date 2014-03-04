using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBargainTypeService : ListEntityDtoServiceBase<BargainType, ListBargainTypeDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBargainTypeService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListBargainTypeDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<BargainType>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListBargainTypeDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        VatRate = x.VatRate,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}