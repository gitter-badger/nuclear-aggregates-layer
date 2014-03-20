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
    public sealed class ListWithdrawalInfoService : ListEntityDtoServiceBase<WithdrawalInfo, ListWithdrawalInfoDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListWithdrawalInfoService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListWithdrawalInfoDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<WithdrawalInfo>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListWithdrawalInfoDto
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    FinishDate = x.FinishDate,
                    PeriodStartDate = x.PeriodStartDate,
                    PeriodEndDate = x.PeriodEndDate,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    StatusEnum = (WithdrawalStatus)x.Status,
                    OwnerCode =  x.OwnerCode,
                    Comment = x.Comment,
                    Status = null,
                    Owner = null,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.Status = x.StatusEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.Owner = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    return x;
                });
        }
    }
}