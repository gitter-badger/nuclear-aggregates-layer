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
    public sealed class ListDenialReasonService : ListEntityDtoServiceBase<DenialReason, ListDenialReasonDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IUserContext _userContext;

        public ListDenialReasonService(
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IEnumerable<ListDenialReasonDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<DenialReason>();

            return query
                .Select(x => new ListDenialReasonDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        TypeEnum = (DenialReasonType)x.Type,
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