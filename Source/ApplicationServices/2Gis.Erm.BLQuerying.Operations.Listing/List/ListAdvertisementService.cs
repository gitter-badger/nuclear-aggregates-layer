using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementService : ListEntityDtoServiceBase<Advertisement, ListAdvertisementDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Advertisement>();

            var firmIdFilter = querySettings.CreateForExtendedProperty<Advertisement, long>(
                "FirmId", firmId => x => x.FirmId == firmId);

            var advertisementTemplateIdFilter = querySettings.CreateForExtendedProperty<Advertisement, long>(
                "AdvertisementTemplateId", advertisementTemplateId => x => x.AdvertisementTemplateId == advertisementTemplateId);

            var isAllowedToWhiteListFilter = querySettings.CreateForExtendedProperty<Advertisement, bool>(
                "isAllowedToWhiteList", isAllowedToWhiteList => x => x.AdvertisementTemplate.IsAllowedToWhiteList == isAllowedToWhiteList);

            var data = query
                .Filter(_filterHelper
                , firmIdFilter
                , advertisementTemplateIdFilter
                , isAllowedToWhiteListFilter)
                .Select(x => new ListAdvertisementDto
                {
                    CreatedOn = x.CreatedOn,
                    Id = x.Id,
                    Name = x.Name,
                    AdvertisementTemplateId = x.AdvertisementTemplateId,
                    AdvertisementTemplateName = x.AdvertisementTemplate.Name,
                    IsSelectedToWhiteList = x.IsSelectedToWhiteList,
                    FirmId = x.FirmId,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}