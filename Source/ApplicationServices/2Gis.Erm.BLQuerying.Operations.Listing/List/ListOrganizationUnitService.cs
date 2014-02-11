﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListOrganizationUnitService : ListEntityDtoServiceBase<OrganizationUnit, ListOrganizationUnitDto>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        public ListOrganizationUnitService(
            IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            IUserContext userContext) 
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        protected override IEnumerable<ListOrganizationUnitDto> GetListData(IQueryable<OrganizationUnit> query, QuerySettings querySettings, out int count)
        {
            var currencyFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, long>(
                 "currencyId", currencyId => x => x.Country.CurrencyId == currencyId);

            var orgUnitFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, long?>(
                 "userId", userId => x => x.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId));

            var firmFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, long?>(
                 "FirmId", firmId => x => x.Firms.Any(y => y.Id == firmId));

             var currentIdentity = _userContext.Identity;

             var restrictByUserFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, bool>(
                "restrictByUser",
                restrictByUser =>
                    {
                        var privelegDepth = GetMaxAccess(
                            _functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.WithdrawalAccess, _userContext.Identity.Code));
                        switch (privelegDepth)
                        {
                            case WithdrawalAccess.None:
                                throw new NotificationException(BLResources.AccessDenied);

                            case WithdrawalAccess.OrganizationUnit:
                                return x => x.UserTerritoriesOrganizationUnits.Any(y => y.UserId == currentIdentity.Code);
                        }

                        return null;
                    });

            // Если указаны фильтры restrictByUser и userId, то первый имеет приоритет (логика перенесена из старого OrganizationUnitController).
            if (restrictByUserFilter != null)
            {
                orgUnitFilter = restrictByUserFilter;
            }

            var franchiseesFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, bool>(
                "restrictByFranchisees",
                restrictByFranchisees =>
                    {
                        if (restrictByFranchisees)
                        {
                            return x => x.UserTerritoriesOrganizationUnits.Any(y => y.UserId == currentIdentity.Code) && x.IsActive && !x.IsDeleted &&
                                        x.ErmLaunchDate != null &&
                                        x.BranchOfficeOrganizationUnits
                                         .FirstOrDefault(y => y.IsPrimary)
                                         .BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Franchisees;
                        }

                        return null;
                    });

            var projectsFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, bool>(
                "restrictByProjects",
                restrictByProjects =>
                    {
                        if (restrictByProjects)
                        {
                            return x => x.Projects.Any(y => y.IsActive);
                        }

                        return null;
                    });

            var branchesMovedToErmFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, bool>(
                "filterByBranchesMovedToErm",
                filterByBranchesMovedToErm => x => x.IsActive && !x.IsDeleted &&
                                                   x.ErmLaunchDate != null &&
                                                   x.BranchOfficeOrganizationUnits
                                                    .FirstOrDefault(y => y.IsPrimary)
                                                    .BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Branch);

            var movedToErmFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, bool>(
                "filterByMovedToErm",
                filterByMovedToErm => x => x.IsActive && !x.IsDeleted &&
                                           x.ErmLaunchDate != null);

            var singlePrimaryBranchOfficeFilter = querySettings.CreateForExtendedProperty<OrganizationUnit, bool>(
                "singlePrimaryBranchOffice",
                singlePrimaryBranchOffice => x => x.IsActive && !x.IsDeleted && 
                    x.BranchOfficeOrganizationUnits.Count(y => y.IsPrimary && y.BranchOffice.IsActive && !y.BranchOffice.IsDeleted) == 1);

            return query
                 .Where(x => !x.IsDeleted)
                 .ApplyFilter(currencyFilter)
                 .ApplyFilter(orgUnitFilter)
                 .ApplyFilter(firmFilter)
                 .ApplyFilter(franchiseesFilter)
                 .ApplyFilter(projectsFilter)
                 .ApplyFilter(branchesMovedToErmFilter)
                 .ApplyFilter(movedToErmFilter)
                 .ApplyFilter(singlePrimaryBranchOfficeFilter)
                 .ApplyQuerySettings(querySettings, out count)
                 .Select(x =>
                         new ListOrganizationUnitDto
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 DgppId = x.DgppId,
                                 CountryId = x.CountryId,
                                 CountryName = x.Country.Name,
                                 FirstEmitDate = x.FirstEmitDate,
                                 ReplicationCode = x.ReplicationCode,
                                 IsDeleted = x.IsDeleted,
                                 IsActive = x.IsActive,
                                 ErmLaunched = x.ErmLaunchDate != null
                             });
         }

         private static WithdrawalAccess GetMaxAccess(int[] accesses)
         {
             if (!accesses.Any())
             {
                 return WithdrawalAccess.None;
             }

             var priorities = new[] { WithdrawalAccess.None, WithdrawalAccess.OrganizationUnit, WithdrawalAccess.Full };

             var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (WithdrawalAccess)x)).Max();
             return priorities[maxPriority];
         }
    }
}
