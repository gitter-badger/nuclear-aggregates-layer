using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementTemplateService : ListEntityDtoServiceBase<AdvertisementTemplate, ListAdvertisementTemplateDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementTemplateService(
            IQuerySettingsProvider querySettingsProvider,
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAdvertisementTemplateDto> List(QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<AdvertisementTemplate>();

            var isPublishedFilter = querySettings.CreateForExtendedProperty<AdvertisementTemplate, bool>(
                "isPublished", isPublished => x => x.IsPublished == isPublished);

            var isUnpublishedFilter = querySettings.CreateForExtendedProperty<AdvertisementTemplate, bool>(
                "isUnpublished", isUnpublished => x => x.IsPublished == !isUnpublished);

            var data = query
                .Filter(_filterHelper
                , isPublishedFilter
                , isUnpublishedFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListAdvertisementTemplateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsPublished = x.IsPublished,
                    CreatedOn = x.CreatedOn,
                })
                .QuerySettings(_filterHelper, querySettings, out count);

            return data;
        }
    }
}
