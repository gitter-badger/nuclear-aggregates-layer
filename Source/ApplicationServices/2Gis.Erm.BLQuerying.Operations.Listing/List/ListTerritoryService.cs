﻿using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListTerritoryService : ListEntityDtoServiceBase<Territory, ListTerritoryDto>
    {
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListTerritoryService(
            IPublicService publicService,
            IFinder finder, FilterHelper filterHelper)
        {
            _publicService = publicService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Territory>();

            Expression<Func<Territory, bool>> restrictToCurrentUserFilter = null;
            if (querySettings.ParentEntityName == EntityName.User && querySettings.ParentEntityId != null)
            {
                var currentUserTerritories = (SelectCurrentUserTerritoriesResponse)_publicService.Handle(new SelectCurrentUserTerritoriesRequest());
                restrictToCurrentUserFilter = x => currentUserTerritories.TerritoryIds.Contains(x.Id);
            }

            var restrictToOrganizationUnit = querySettings.CreateForExtendedProperty<Territory, long>("restrictToOrganizationUnit",
                organizationUnit =>
                    {
                        var request = new SelectOrganizationUnitTerritoriesRequest { OrganizationUnitId = organizationUnit };
                        var territories = ((SelectOrganizationUnitTerritoriesResponse)_publicService.Handle(request)).TerritoryIds;

                        return x => territories.Contains(x.Id);
                    });

            return query
                .Filter(_filterHelper, restrictToCurrentUserFilter, restrictToOrganizationUnit)
                .Select(x => new ListTerritoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    IsActive = x.IsActive,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}