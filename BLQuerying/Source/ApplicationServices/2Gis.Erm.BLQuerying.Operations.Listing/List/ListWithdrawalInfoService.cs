using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListWithdrawalInfoService : ListEntityDtoServiceBase<WithdrawalInfo, ListWithdrawalInfoDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListWithdrawalInfoService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IQuery query,
            FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<WithdrawalInfo>();

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
                    OwnerCode = x.OwnerCode,
                    Comment = x.Comment,
                    Status = x.Status.ToStringLocalizedExpression(),
                    AccountingMethod = x.AccountingMethod.ToStringLocalizedExpression(),
                    Owner = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListWithdrawalInfoDto dto)
        {
            dto.Owner = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}