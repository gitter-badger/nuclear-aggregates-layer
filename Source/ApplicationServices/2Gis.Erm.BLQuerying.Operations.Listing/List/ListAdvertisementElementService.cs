using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
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
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<AdvertisementElement>();

            return query
                .Select(x => new ListAdvertisementElementDto
                {
                    Id = x.Id,
                    AdvertisementElementTemplateId = x.AdvertisementElementTemplate.Id,
                    AdvertisementElementTemplateName = x.AdvertisementElementTemplate.Name,
                    RestrictionTypeEnum = (AdvertisementElementRestrictionType)x.AdvertisementElementTemplate.RestrictionType,
                    IsRequired = x.AdvertisementElementTemplate.IsRequired,
                    AdvertisementId = x.AdvertisementId,
                    RestrictionType = null,
                })
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                {
                    x.RestrictionType = x.RestrictionTypeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    return x;
                });
        }
    }
}