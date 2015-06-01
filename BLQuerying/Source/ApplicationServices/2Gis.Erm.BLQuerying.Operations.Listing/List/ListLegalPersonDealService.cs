using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListLegalPersonDealService : ListEntityDtoServiceBase<LegalPersonDeal, ListLegalPersonDealDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListLegalPersonDealService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<LegalPersonDeal>();

            return query
                .Filter(_filterHelper)
                .Select(x => new ListLegalPersonDealDto
                    {
                        Id = x.Id,
                        LegalPersonId = x.LegalPersonId,
                        DealId = x.DealId,
                        LegalName = x.LegalPerson.LegalName,
                        IsDeleted = x.IsDeleted,
                        IsMain = x.IsMain
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}