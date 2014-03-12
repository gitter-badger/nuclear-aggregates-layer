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
    public sealed class ListReleaseInfoService : ListEntityDtoServiceBase<ReleaseInfo, ListReleaseInfoDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListReleaseInfoService(
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

        protected override IEnumerable<ListReleaseInfoDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<ReleaseInfo>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new
                    {
                        x.Id,
                        x.StartDate,
                        x.FinishDate,
                        x.PeriodStartDate,
                        x.PeriodEndDate,
                        x.OrganizationUnitId,
                        OrganizationUnitName = x.OrganizationUnit.Name,
                        x.IsBeta,
                        Status = (ReleaseStatus)x.Status,
                        x.OwnerCode,
                        x.Comment,
                    })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                        new ListReleaseInfoDto
                            {
                                Id = x.Id,
                                StartDate = x.StartDate,
                                FinishDate = x.FinishDate,
                                PeriodStartDate = x.PeriodStartDate,
                                PeriodEndDate = x.PeriodEndDate,
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnitName,
                                OperationType =
                                    x.IsBeta
                                        ? ReleaseInfoOperationType.Beta.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo)
                                        : ReleaseInfoOperationType.Release.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OwnerCode = x.OwnerCode,
                                Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                Comment = x.Comment
                            });
        }
    }
}