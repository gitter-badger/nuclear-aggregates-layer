using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPriceService : ListEntityDtoServiceBase<Price, ListPriceDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPriceService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPriceDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Price>();

            var excludeIdFilter = querySettings.CreateForExtendedProperty<Price, long>(
                "excludeId",
                excludeId => x => x.Id != excludeId);

            return query
                .Filter(_filterHelper, excludeIdFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                    {
                        x.Id,
                        x.CreateDate,
                        x.PublishDate,
                        x.BeginDate,
                        OrganizationUnitName = x.OrganizationUnit.Name,
                        CurrencyName = x.Currency.Name,
                        x.IsPublished,
                        x.OrganizationUnitId,
                    })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                        new ListPriceDto
                            {
                                Id = x.Id,
                                CreateDate = x.CreateDate,
                                PublishDate = x.PublishDate,
                                BeginDate = x.BeginDate,
                                OrganizationUnitName = x.OrganizationUnitName,
                                CurrencyName = x.CurrencyName,
                                IsPublished = x.IsPublished,
                                Name = string.Format("{0} ({1})", x.BeginDate.ToShortDateString(), x.OrganizationUnitName),
                                OrganizationUnitId = x.OrganizationUnitId,
                            });
        }
    }
}