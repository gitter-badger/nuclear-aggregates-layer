﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
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
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
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
                .Select(x => new ListAdvertisementTemplateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsPublished = x.IsPublished,
                    CreatedOn = x.CreatedOn,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}
