using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdsTemplatesAdsElementTemplateService : ListEntityDtoServiceBase<AdsTemplatesAdsElementTemplate, ListAdsTemplatesAdsElementTemplateDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAdsTemplatesAdsElementTemplateService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<AdsTemplatesAdsElementTemplate>();

            return query
                .Select(x => new ListAdsTemplatesAdsElementTemplateDto
                    {
                        Id = x.Id,
                        AdsElementTemplateId = x.AdsElementTemplateId,
                        AdsTemplateId = x.AdsTemplateId,
                        AdsTemplateName = x.AdvertisementTemplate.Name,
                        AdsElementTemplateName = x.AdvertisementElementTemplate.Name,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}