using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListLimitService : ListEntityDtoServiceBase<Limit, ListLimitDto>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly FilterHelper _filterHelper;

        public ListLimitService(
            IFinder finder,
            IUserContext userContext,
            ISecurityServiceUserIdentifier userIdentifierService,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _userContext = userContext;
            _userIdentifierService = userIdentifierService;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<Limit>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var myBranchFilter = querySettings.CreateForExtendedProperty<Limit, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Account.LegalPerson.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            return query
                .Filter(_filterHelper, myBranchFilter)
                .Select(x => new ListLimitDto
                {
                    Id = x.Id,
                    BranchOfficeId = x.Account.BranchOfficeOrganizationUnit.BranchOfficeId,
                    BranchOfficeName = x.Account.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    LegalPersonName = x.Account.LegalPerson.LegalName,
                    CreatedOn = x.CreatedOn,
                    CloseDate = x.CloseDate,
                    StartPeriodDate = x.StartPeriodDate,
                    EndPeriodDate = x.EndPeriodDate,
                    Amount = x.Amount,
                    ClientId = x.Account.LegalPerson.ClientId,
                    ClientName = x.Account.LegalPerson.Client.Name,
                    OwnerCode = x.OwnerCode,
                    InspectorCode = x.InspectorCode,
                    StatusEnum = x.Status,
                    AccountId = x.AccountId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    LegalPersonId = x.Account.LegalPersonId,
                    OwnerName = null,
                    Status = x.Status.ToStringLocalizedExpression(),
                    InspectorName = null,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListLimitDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
            dto.InspectorName = _userIdentifierService.GetUserInfo(dto.InspectorCode).DisplayName;
        }
    }
}