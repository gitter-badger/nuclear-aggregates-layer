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
    public class ListReleaseInfoService : ListEntityDtoServiceBase<ReleaseInfo, ListReleaseInfoDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListReleaseInfoService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListReleaseInfoDto> GetListData(IQueryable<ReleaseInfo> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .Where(x => !x.IsDeleted)
                .ApplyQuerySettings(querySettings, out count)
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
                .AsEnumerable()
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
                                        ? ReleaseInfoOperationType.Beta.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo)
                                        : ReleaseInfoOperationType.Release.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OwnerCode = x.OwnerCode,
                                Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                Comment = x.Comment
                            });
        }
    }
}