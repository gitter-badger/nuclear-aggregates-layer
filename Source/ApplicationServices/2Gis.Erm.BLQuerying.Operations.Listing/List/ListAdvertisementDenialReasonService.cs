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
    public sealed class ListAdvertisementDenialReasonService : ListEntityDtoServiceBase<AdvertisementDenialReason, ListAdvertisementDenialReasonDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IUserContext _userContext;

        public ListAdvertisementDenialReasonService(
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IEnumerable<ListAdvertisementDenialReasonDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AdvertisementDenialReason>();

            return query
                .Select(x => new ListAdvertisementDenialReasonDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        TypeEnum = (AdvertisementRestrictionType)x.Type,
                        CreatedOn = x.CreatedOn,
                        IsActive = x.IsActive
                    })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                    {
                        x.Type = x.TypeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                        return x;
                    });
        }
    }
}