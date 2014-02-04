using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListPriceService : ListEntityDtoServiceBase<Price, ListPriceDto>
    {
        public ListPriceService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListPriceDto> GetListData(IQueryable<Price> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.CreateDate,
                        x.PublishDate,
                        x.BeginDate,
                        OrganizationUnitName = x.OrganizationUnit.Name,
                        CurrencyName = x.Currency.Name,
                        x.IsPublished
                    })
                .AsEnumerable()
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
                                Name = string.Format("{0} ({1})", x.BeginDate.ToShortDateString(), x.OrganizationUnitName)
                            });
        }
    }
}