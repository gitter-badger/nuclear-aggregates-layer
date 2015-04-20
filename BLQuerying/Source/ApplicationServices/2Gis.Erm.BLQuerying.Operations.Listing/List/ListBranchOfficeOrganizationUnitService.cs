using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBranchOfficeOrganizationUnitService : ListEntityDtoServiceBase<BranchOfficeOrganizationUnit, ListBranchOfficeOrganizationUnitDto>
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;
        private readonly IUserReadModel _userReadModel;

        public ListBranchOfficeOrganizationUnitService(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            IUserContext userContext,
            FilterHelper filterHelper,
            IUserReadModel userReadModel)
        {
            _functionalAccessService = functionalAccessService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
            _userReadModel = userReadModel;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<BranchOfficeOrganizationUnit>();

            var filter = querySettings.CreateForExtendedProperty<BranchOfficeOrganizationUnit, long>(
                "OrderSourceOrganizationUnitId",
                sourceOrganizationUnitId =>
                    {
                        var userId = _userContext.Identity.Code;

                        bool primaryRequired;
                        querySettings.TryGetExtendedProperty("Primary", out primaryRequired);
                        if (_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection, userId))
                        {
                            return x => x.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId) && (!primaryRequired || x.IsPrimary);
                        }
                        else
                        {
                            var branchOfficeIds = _userReadModel.GetUserBranchOffices(userId);
                            if (branchOfficeIds.Any())
                            {
                                return x => branchOfficeIds.Contains(x.BranchOfficeId);
                            }

                            return x => x.OrganizationUnitId == sourceOrganizationUnitId && (!primaryRequired || x.IsPrimary);
                        }
                    });

            var data = query
                .Filter(_filterHelper, filter)
                .Select(x => new ListBranchOfficeOrganizationUnitDto
                {
                    Id = x.Id,
                    BranchOfficeId = x.BranchOfficeId,
                    OrganizationUnitId = x.OrganizationUnitId,
                    ShortLegalName = x.ShortLegalName,
                    BranchOfficeName = x.BranchOffice.Name,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    IsPrimary = x.IsPrimary,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    OrganizationUnitIsDeleted = x.OrganizationUnit.IsDeleted,
                    BranchOfficeIsDeleted = x.BranchOffice.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}