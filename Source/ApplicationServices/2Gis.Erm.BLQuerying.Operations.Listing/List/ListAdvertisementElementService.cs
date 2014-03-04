using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementElementService : ListEntityDtoServiceBase<AdvertisementElement, ListAdvertisementElementDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementElementService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAdvertisementElementDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AdvertisementElement>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                {
                    x.Id,
                    AdvertisementElementTemplateId = x.AdvertisementElementTemplate.Id,
                    AdvertisementElementTemplateName = x.AdvertisementElementTemplate.Name,
                    x.AdvertisementElementTemplate.RestrictionType,
                    x.AdvertisementElementTemplate.IsRequired
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListAdvertisementElementDto
                {
                    Id = x.Id,
                    AdvertisementElementTemplateId = x.AdvertisementElementTemplateId,
                    AdvertisementElementTemplateName = x.AdvertisementElementTemplateName,
                    IsRequired = x.IsRequired,
                    RestrictionType = ((AdvertisementElementRestrictionType)x.RestrictionType).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo)
                });
        }
    }
}