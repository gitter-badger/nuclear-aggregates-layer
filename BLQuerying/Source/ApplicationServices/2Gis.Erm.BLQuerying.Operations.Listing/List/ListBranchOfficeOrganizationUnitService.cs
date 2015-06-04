﻿using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBranchOfficeOrganizationUnitService : ListEntityDtoServiceBase<BranchOfficeOrganizationUnit, ListBranchOfficeOrganizationUnitDto>
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IQuery _query;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;
        private readonly IUserReadModel _userReadModel;

        public ListBranchOfficeOrganizationUnitService(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IQuery query,
            IUserContext userContext,
            FilterHelper filterHelper,
            IUserReadModel userReadModel)
        {
            _functionalAccessService = functionalAccessService;
            _query = query;
            _userContext = userContext;
            _filterHelper = filterHelper;
            _userReadModel = userReadModel;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<BranchOfficeOrganizationUnit>();

            var filter = querySettings.CreateForExtendedProperty<BranchOfficeOrganizationUnit, long>(
                "OrderSourceOrganizationUnitId",
                sourceOrganizationUnitId =>
                {
                    var userId = _userContext.Identity.Code;
                    bool primaryRequired;

                    if (!querySettings.TryGetExtendedProperty("Primary", out primaryRequired))
                    {
                        primaryRequired = false;
                        }

                    Expression<Func<BranchOfficeOrganizationUnit, bool>> primaryFilter = x => x.IsPrimary;
                    if (_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection, userId))
                    {
                        Expression<Func<BranchOfficeOrganizationUnit, bool>> functionalPrivilegeFilter =
                            x => x.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
                        return primaryRequired
                                   ? primaryFilter.And(functionalPrivilegeFilter)
                                   : functionalPrivilegeFilter;
                    }
                    else
                            {
                        Expression<Func<BranchOfficeOrganizationUnit, bool>> sourceOrganizationUnitFilter = x => x.OrganizationUnitId == sourceOrganizationUnitId;
                        var branchOfficeIds = _userReadModel.GetUserBranchOffices(userId);
                        if (branchOfficeIds.Any())
                                {
                            return sourceOrganizationUnitFilter.And(x => branchOfficeIds.Contains(x.BranchOfficeId));
                            }

                        return primaryRequired
                                   ? sourceOrganizationUnitFilter.And(primaryFilter)
                                   : sourceOrganizationUnitFilter;
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