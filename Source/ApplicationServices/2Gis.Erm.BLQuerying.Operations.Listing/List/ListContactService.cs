using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListContactService : ListEntityDtoServiceBase<Contact, ListContactDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListContactService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext) 
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListContactDto> GetListData(IQueryable<Contact> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
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
                .AsEnumerable()
                .Select(x =>
                        new ListContactDto
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
                                AccountRole = ((AccountRole)x.AccountRole).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo)
                            });
        }
    }
}