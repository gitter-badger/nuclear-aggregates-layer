using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBargainService : ListEntityDtoServiceBase<Bargain, ListBargainDto>
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public ListBargainService(
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext,
            ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Bargain>();

            var hasAdvertisementAgencyManagementPrivilege =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, _userContext.Identity.Code);
            Expression<Func<Bargain, bool>> agentBargainsFilter = x => hasAdvertisementAgencyManagementPrivilege || x.BargainKind != (int)BargainKind.Agent;

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper, agentBargainsFilter)
                .Select(x => new ListBargainDto
                {
                    Id = x.Id,
                    Number = x.Number,
                    LegalPersonId = x.CustomerLegalPersonId,
                    LegalPersonLegalName = x.LegalPerson.LegalName,
                    LegalPersonLegalAddress = x.LegalPerson.LegalAddress,
                    BranchOfficeId = x.BranchOfficeOrganizationUnit.BranchOfficeId,
                    BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                    CreatedOn = x.CreatedOn,
                    BargainEndDate = x.BargainEndDate,
                    OwnerCode = x.OwnerCode,
                    ClientId = x.LegalPerson.ClientId,
                    ClientName = x.LegalPerson.Client.Name,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ExecutorBranchOfficeId = x.ExecutorBranchOfficeId,
                    BargainKindEnum = (BargainKind)x.BargainKind,
                    BargainKind = ((BargainKind)x.BargainKind).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}
