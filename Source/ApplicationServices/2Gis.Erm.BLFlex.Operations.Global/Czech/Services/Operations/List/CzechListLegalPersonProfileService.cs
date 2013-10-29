﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Metadata;
using DoubleGis.Erm.BL.Operations.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Core.Services;
using DoubleGis.Erm.Core.Services.Operations.Concrete.List.DTO;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Services.Operations.List.Czech
{
    public class CzechListLegalPersonProfileService : ListEntityDtoServiceBase<LegalPersonProfile, CzechListLegalPersonProfileDto>, ICzechAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public CzechListLegalPersonProfileService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<CzechListLegalPersonProfileDto> GetListData(
            IQueryable<LegalPersonProfile> query,
            QuerySettings querySettings,
            ListFilterManager filterManager,
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
                        new CzechListLegalPersonProfileDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsMainProfile = x.IsMainProfile,
                            OwnerCode = x.OwnerCode,
                            OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                            CreatedOn = x.CreatedOn,
                        });
        }
    }
}
