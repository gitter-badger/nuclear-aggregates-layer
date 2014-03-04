using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.List
{
    public sealed class CzechListLegalPersonProfileService : ListEntityDtoServiceBase<LegalPersonProfile, CzechListLegalPersonProfileDto>, ICzechAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public CzechListLegalPersonProfileService(
            IQuerySettingsProvider querySettingsProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<CzechListLegalPersonProfileDto> List(QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<LegalPersonProfile>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
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
                .QuerySettings(_filterHelper, querySettings, out count)
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
