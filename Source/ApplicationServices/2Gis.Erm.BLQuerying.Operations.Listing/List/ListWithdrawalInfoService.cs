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
    public class ListWithdrawalInfoService : ListEntityDtoServiceBase<WithdrawalInfo, ListWithdrawalInfoDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListWithdrawalInfoService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListWithdrawalInfoDto> GetListData(IQueryable<WithdrawalInfo> query, QuerySettings querySettings, out int count)
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
                        Status = (WithdrawalStatus)x.Status,
                        x.OwnerCode,
                        x.Comment,
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListWithdrawalInfoDto
                            {
                                Id = x.Id,
                                StartDate = x.StartDate,
                                FinishDate = x.FinishDate,
                                PeriodStartDate = x.PeriodStartDate,
                                PeriodEndDate = x.PeriodEndDate,
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnitName,
                                Status = x.Status.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                                OwnerCode = x.OwnerCode,
                                Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                                Comment = x.Comment
                            });
        }
    }
}