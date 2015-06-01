using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPriceService : ListEntityDtoServiceBase<Price, ListPriceDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListPriceService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Price>();

            return query
                .Select(x => new ListPriceDto
                {
                    Id = x.Id,
                    CreateDate = x.CreateDate,
                    PublishDate = x.PublishDate,
                    BeginDate = x.BeginDate,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    CurrencyName = x.Currency.Name,
                    IsPublished = x.IsPublished,
                    OrganizationUnitId = x.OrganizationUnitId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    Name = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListPriceDto dto)
        {
            dto.Name = string.Format("{0} ({1})", dto.BeginDate.ToShortDateString(), dto.OrganizationUnitName);
        }
    }
}