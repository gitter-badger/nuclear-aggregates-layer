using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListContactService : ListEntityDtoServiceBase<Contact, ListContactDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListContactService(
            IQuerySettingsProvider querySettingsProvider, 
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListContactDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Contact>();

            return query
            .DefaultFilter(_filterHelper, querySettings)
            .Select(x => new
            {
                x.Id,
                x.FullName,
                x.ClientId,
                Client = x.Client.Name,
                x.JobTitle,
                x.MainPhoneNumber,
                x.MobilePhoneNumber,
                x.Website,
                x.HomeEmail,
                x.WorkEmail,
                x.OwnerCode,
                CreateDate = x.CreatedOn,
                x.WorkAddress,
                x.AccountRole
            })
            .QuerySettings(_filterHelper, querySettings, out count)
            .Select(x => new ListContactDto
            {
                Id = x.Id,
                FullName = x.FullName,
                ClientId = x.ClientId,
                Client = x.Client,
                JobTitle = x.JobTitle,
                MainPhoneNumber = x.MainPhoneNumber,
                MobilePhoneNumber = x.MobilePhoneNumber,
                Website = x.Website,
                HomeEmail = x.HomeEmail,
                WorkEmail = x.WorkEmail,
                OwnerCode = x.OwnerCode,
                Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                CreateDate = x.CreateDate,
                WorkAddress = x.WorkAddress,
                AccountRole = ((AccountRole)x.AccountRole).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo)
            });
        }
    }
}