using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.List
{
    public sealed class CzechListLegalPersonProfileService : ListEntityDtoServiceBase<LegalPersonProfile, CzechListLegalPersonProfileDto>, ICzechAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public CzechListLegalPersonProfileService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IQuery query, FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<LegalPersonProfile>();

            return query
                .Select(x => new CzechListLegalPersonProfileDto
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

        protected override void Transform(CzechListLegalPersonProfileDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}
