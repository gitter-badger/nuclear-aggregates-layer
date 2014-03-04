using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListCurrencyRateService : ListEntityDtoServiceBase<CurrencyRate, ListCurrencyRateDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListCurrencyRateService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListCurrencyRateDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<CurrencyRate>();

            var utcNow = DateTime.UtcNow;

            return query
            .Where(x => !x.IsDeleted)
            .DefaultFilter(_filterHelper, querySettings)
            .Select(x => new
            {
                x.Id,
                x.CurrencyId,
                CurrencyName = x.Currency.Name,
                x.BaseCurrencyId,
                BaseCurrencyName = x.BaseCurrency.Name,
                x.Rate,
                x.CreatedOn,
                x.IsDeleted,
                IsCurrent = query
                            .Where(y => !y.IsDeleted && y.CreatedOn <= utcNow && y.CurrencyId == x.CurrencyId)
                            .OrderByDescending(y => y.CreatedOn)
                            .Select(y => y.Id)
                            .FirstOrDefault() == x.Id
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x => new ListCurrencyRateDto
            {
                Id = x.Id,
                CurrencyId = x.CurrencyId,
                CurrencyName = x.CurrencyName,
                BaseCurrencyId = x.BaseCurrencyId,
                BaseCurrencyName = x.BaseCurrencyName,
                Rate = x.Rate,
                CreatedOn = x.CreatedOn,
                IsDeleted = x.IsDeleted,
                IsCurrent = x.IsCurrent
            });
        }
    }
}