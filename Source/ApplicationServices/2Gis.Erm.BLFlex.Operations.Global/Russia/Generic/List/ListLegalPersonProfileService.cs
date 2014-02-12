﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.List
{
    public class ListLegalPersonProfileService : ListEntityDtoServiceBase<LegalPersonProfile, ListLegalPersonProfileDto>, IRussiaAdapted, ICyprusAdapted, IChileAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListLegalPersonProfileService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListLegalPersonProfileDto> GetListData(
            IQueryable<LegalPersonProfile> query, 
            QuerySettings querySettings,
            out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                                 {
                                     x.Id,
                                     x.Name,
                                     x.IsMainProfile,
                                     x.OwnerCode,
                                     x.CreatedOn,
                                     x.IsDeleted,
                                     x.IsActive,
                                 })
                .AsEnumerable()
                .Select(x =>
                        new ListLegalPersonProfileDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                IsMainProfile = x.IsMainProfile,
                                OwnerCode = x.OwnerCode,
                                CreatedOn = x.CreatedOn,
                                OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                IsDeleted = x.IsDeleted,
                                IsActive = x.IsActive
                            });
        }
    }
}
