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
    public sealed class ListAdvertisementTemplateService : ListEntityDtoServiceBase<AdvertisementTemplate, ListAdvertisementTemplateDto>
    {
        public ListAdvertisementTemplateService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListAdvertisementTemplateDto> GetListData(
            IQueryable<AdvertisementTemplate> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
            out int count)
        {
            var isPublishedFilter = filterManager.CreateForExtendedProperty<AdvertisementTemplate, bool>(
                "isPublished", isPublished => x => x.IsPublished == isPublished);

            var isUnpublishedFilter = filterManager.CreateForExtendedProperty<AdvertisementTemplate, bool>(
                "isUnpublished", isUnpublished => x => x.IsPublished == !isUnpublished);

            var data = query
                .ApplyFilter(isPublishedFilter)
                .ApplyFilter(isUnpublishedFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x =>
                        new ListAdvertisementTemplateDto
                            {
                                CreatedOn = x.CreatedOn,
                                Id = x.Id,
                                Name = x.Name,
                                IsPublished = x.IsPublished
                            });

            return data;
        }
    }
}
