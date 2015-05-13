using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementElementService : ListEntityDtoServiceBase<AdvertisementElement, ListAdvertisementElementDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementElementService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<AdvertisementElement>();

            return query
                .Select(x => new ListAdvertisementElementDto
                {
                    Id = x.Id,
                    AdvertisementElementTemplateId = x.AdvertisementElementTemplate.Id,
                    AdvertisementElementTemplateName = x.AdvertisementElementTemplate.Name,
                    IsRequired = x.AdvertisementElementTemplate.IsRequired,
                    AdvertisementId = x.AdvertisementId,
                    RestrictionType = x.AdvertisementElementTemplate.RestrictionType.ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}