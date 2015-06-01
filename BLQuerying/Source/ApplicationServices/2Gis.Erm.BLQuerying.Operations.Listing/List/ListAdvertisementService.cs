using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementService : ListEntityDtoServiceBase<Advertisement, ListAdvertisementDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Advertisement>();

            var data = query
                .Select(x => new ListAdvertisementDto
                {
                    CreatedOn = x.CreatedOn,
                    Id = x.Id,
                    Name = x.Name,
                    AdvertisementTemplateId = x.AdvertisementTemplateId,
                    AdvertisementTemplateName = x.AdvertisementTemplate.Name,
                    IsSelectedToWhiteList = x.IsSelectedToWhiteList,
                    IsAllowedToWhiteList = x.AdvertisementTemplate.IsAllowedToWhiteList,
                    FirmId = x.FirmId,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}