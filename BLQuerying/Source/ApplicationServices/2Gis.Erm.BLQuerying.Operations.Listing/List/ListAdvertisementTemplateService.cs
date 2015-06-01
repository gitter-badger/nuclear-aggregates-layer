using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementTemplateService : ListEntityDtoServiceBase<AdvertisementTemplate, ListAdvertisementTemplateDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementTemplateService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<AdvertisementTemplate>();

            var data = query
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
