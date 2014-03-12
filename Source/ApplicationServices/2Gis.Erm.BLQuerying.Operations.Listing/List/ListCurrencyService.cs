﻿using System.Collections.Generic;
using System.Linq;

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
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListCurrencyDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Currency>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
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
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}