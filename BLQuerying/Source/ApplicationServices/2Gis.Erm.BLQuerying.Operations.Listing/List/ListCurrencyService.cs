using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCurrencyService : ListEntityDtoServiceBase<Currency, ListCurrencyDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListCurrencyService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<Currency>();

            return query
                .Select(x => new ListCurrencyDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Symbol = x.Symbol,
                        ISOCode = x.ISOCode,
                        IsBase = x.IsBase,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}