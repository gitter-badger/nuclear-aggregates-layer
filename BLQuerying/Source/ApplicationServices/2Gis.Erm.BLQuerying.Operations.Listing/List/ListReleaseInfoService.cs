using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListReleaseInfoService : ListEntityDtoServiceBase<ReleaseInfo, ListReleaseInfoDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListReleaseInfoService(
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
            var query = _finder.For<ReleaseInfo>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListReleaseInfoDto
                {
                    Id = x.Id,
                    StartDate = x.StartDate,
                    FinishDate = x.FinishDate,
                    PeriodStartDate = x.PeriodStartDate,
                    PeriodEndDate = x.PeriodEndDate,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    IsBeta = x.IsBeta,
                    StatusEnum = x.Status,
                    OwnerCode = x.OwnerCode,
                    Comment = x.Comment,
                    Status = x.Status.ToStringLocalizedExpression(),
                    Owner = null,
                    OperationType = (x.IsBeta ? ReleaseInfoOperationType.Beta : ReleaseInfoOperationType.Release).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListReleaseInfoDto dto)
        {
            dto.Owner = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }

        // localization-only enum может не содержать None значения
        private enum ReleaseInfoOperationType
        {
            Beta,
            Release,
        }
    }
}