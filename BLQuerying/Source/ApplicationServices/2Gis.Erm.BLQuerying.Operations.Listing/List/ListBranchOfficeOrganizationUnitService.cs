using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
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

        public ListBranchOfficeOrganizationUnitService(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _functionalAccessService = functionalAccessService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<BranchOfficeOrganizationUnit>();

            var filter = querySettings.CreateForExtendedProperty<BranchOfficeOrganizationUnit, long, bool>(
                "userId", 
                "restrictByFP",
                (userId, restrictByPrivelege) =>
                {
                    if (restrictByPrivelege)
                    {
                        var hasPrivilege = _functionalAccessService
                            .HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection, _userContext.Identity.Code);
                        if (!hasPrivilege)
                        {
                            return null;
                        }

                        Expression<Func<BranchOfficeOrganizationUnit, bool>> filterExpression =
                            x => x.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);

                        return filterExpression;
                    }
                    return null;
                });

            if (filter == null)
            {
                filter = querySettings.CreateForExtendedProperty<BranchOfficeOrganizationUnit, long, bool>(
                    "sourceOrganizationUnitId", 
                    "restrictByFP",
                    (sourceOrganizationUnitId, restrictByPrivelege) =>
                        {
                            Expression<Func<BranchOfficeOrganizationUnit, bool>> filterExpression =
                                x => x.OrganizationUnitId == sourceOrganizationUnitId;

                            if (restrictByPrivelege)
                            {
                                var hasPrivilege = _functionalAccessService
                                    .HasFunctionalPrivilegeGranted(
                                        FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection,
                                        _userContext.Identity.Code);

                                if (!hasPrivilege)
                                {
                                    return filterExpression;
                                }
                                return null;
                            }

                            return filterExpression;
                        });
            }

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