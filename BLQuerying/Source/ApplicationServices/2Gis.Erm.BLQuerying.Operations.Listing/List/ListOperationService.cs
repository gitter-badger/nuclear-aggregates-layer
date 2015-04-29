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
    public sealed class ListOperationService : ListEntityDtoServiceBase<Operation, ListOperationDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListOperationService(
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
            var query = _finder.For<Operation>();

            return query
                .Select(x => new ListOperationDto
                {
                    Id = x.Id,
                    StartTime = x.StartTime,
                    FinishTime = x.FinishTime,
                    TypeEnum = x.Type,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    OwnerCode = x.OwnerCode,
                    Description = x.Description,
                    Owner = null,
                    Status = x.Status.ToStringLocalizedExpression(),
                    Type = x.Type.ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListOperationDto dto)
        {
            dto.Owner = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}
