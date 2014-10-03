﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.List
{
    public class ListLegalPersonProfileService : ListEntityDtoServiceBase<LegalPersonProfile, ListLegalPersonProfileDto>, IRussiaAdapted, ICyprusAdapted,
                                                 IChileAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListLegalPersonProfileService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<LegalPersonProfile>();

            return query
                .Select(x => new ListLegalPersonProfileDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                IsMainProfile = x.IsMainProfile,
                                OwnerCode = x.OwnerCode,
                                CreatedOn = x.CreatedOn,
                                IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    LegalPersonId = x.LegalPersonId,
                    OwnerName = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListLegalPersonProfileDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}
