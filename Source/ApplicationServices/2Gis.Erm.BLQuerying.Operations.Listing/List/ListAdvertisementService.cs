using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementService : ListEntityDtoServiceBase<Advertisement, ListAdvertisementDto>
    {
        public ListAdvertisementService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListAdvertisementDto> GetListData(IQueryable<Advertisement> query, QuerySettings querySettings, out int count)
        {
            var firmIdFilter = querySettings.CreateForExtendedProperty<Advertisement, long>(
                "FirmId", firmId => x => x.FirmId == firmId);

            var advertisementTemplateIdFilter = querySettings.CreateForExtendedProperty<Advertisement, long>(
                "AdvertisementTemplateId", advertisementTemplateId => x => x.AdvertisementTemplateId == advertisementTemplateId);

            var isAllowedToWhiteListFilter = querySettings.CreateForExtendedProperty<Advertisement, bool>(
                "isAllowedToWhiteList", isAllowedToWhiteList => x => x.AdvertisementTemplate.IsAllowedToWhiteList == isAllowedToWhiteList);

            var data = query
                .ApplyFilter(firmIdFilter)
                .ApplyFilter(advertisementTemplateIdFilter)
                .ApplyFilter(isAllowedToWhiteListFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x =>
                        new ListAdvertisementDto
                            {
                                CreatedOn = x.CreatedOn,
                                Id = x.Id,
                                Name = x.Name,
                                AdvertisementTemplateId = x.AdvertisementTemplateId,
                                AdvertisementTemplateName = x.AdvertisementTemplate.Name,
                                IsSelectedToWhiteList = x.IsSelectedToWhiteList
                            });

            return data;
        }
    }
}