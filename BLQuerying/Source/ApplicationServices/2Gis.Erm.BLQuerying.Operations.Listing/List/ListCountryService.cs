using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCountryService : ListEntityDtoServiceBase<Country, ListCountryDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListCountryService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Country>();

            return query
                .Select(x => new ListCountryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsoCode = x.IsoCode,
                        CurrencyId = x.CurrencyId,
                        CurrencyName = x.Currency.Name,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}