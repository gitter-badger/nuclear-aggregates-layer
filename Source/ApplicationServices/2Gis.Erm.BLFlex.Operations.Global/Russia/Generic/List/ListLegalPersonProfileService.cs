using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.List
{
    public sealed class ListLegalPersonProfileService : ListEntityDtoServiceBase<LegalPersonProfile, ListLegalPersonProfileDto>, IRussiaAdapted, ICyprusAdapted, IChileAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListLegalPersonProfileService(
            IQuerySettingsProvider querySettingsProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListLegalPersonProfileDto> List(QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<LegalPersonProfile>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
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
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    return x;
                });
        }
    }
}
