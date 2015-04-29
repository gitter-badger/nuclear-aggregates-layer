using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCountryService : ListEntityDtoServiceBase<Country, ListCountryDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListCountryService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<Country>();

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