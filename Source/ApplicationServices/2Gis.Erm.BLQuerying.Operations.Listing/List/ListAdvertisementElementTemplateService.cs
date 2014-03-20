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
    public sealed class ListAdvertisementElementTemplateService : ListEntityDtoServiceBase<AdvertisementElementTemplate, ListAdvertisementElementTemplateDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementElementTemplateService(
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAdvertisementElementTemplateDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AdvertisementElementTemplate>();

            return query
                .Select(x => new ListAdvertisementElementTemplateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    RestrictionTypeEnum = (AdvertisementElementRestrictionType)x.RestrictionType,
                    IsDeleted = x.IsDeleted,
                    RestrictionType = null,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.RestrictionType = x.RestrictionTypeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    return x;
                });
        }
    }
}