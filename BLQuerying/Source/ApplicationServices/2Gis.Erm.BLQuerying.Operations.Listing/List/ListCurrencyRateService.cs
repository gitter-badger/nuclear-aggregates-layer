using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCurrencyRateService : ListEntityDtoServiceBase<CurrencyRate, ListCurrencyRateDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListCurrencyRateService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<CurrencyRate>();

            var utcNow = DateTime.UtcNow;

            return query
            .Where(x => !x.IsDeleted)
            .Select(x => new ListCurrencyRateDto
            {
                Id = x.Id,
                CurrencyId = x.CurrencyId,
                CurrencyName = x.Currency.Name,
                BaseCurrencyId = x.BaseCurrencyId,
                BaseCurrencyName = x.BaseCurrency.Name,
                Rate = x.Rate,
                CreatedOn = x.CreatedOn,
                IsDeleted = x.IsDeleted,
                IsCurrent = query
                            .Where(y => !y.IsDeleted && y.CreatedOn <= utcNow && y.CurrencyId == x.CurrencyId)
                            .OrderByDescending(y => y.CreatedOn)
                            .Select(y => y.Id)
                            .FirstOrDefault() == x.Id,
            })
            .QuerySettings(_filterHelper, querySettings);
        }
    }
}