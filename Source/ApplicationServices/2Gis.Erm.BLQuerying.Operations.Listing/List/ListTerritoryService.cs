using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
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
            IQuerySettingsProvider querySettingsProvider, 
            IPublicService publicService,
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _publicService = publicService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListTerritoryDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Territory>();

            var restrictToCurrentUserFilter = querySettings
                .CreateByParentEntity<Territory>(EntityName.User,
                                                 () =>
                                                     {
                                                         var currentUserTerritories = (SelectCurrentUserTerritoriesResponse)
                                                                                      _publicService.Handle(new SelectCurrentUserTerritoriesRequest());
                                                         return x => currentUserTerritories.TerritoryIds.Contains(x.Id);
                                                     });

            var restrictToOrganizationUnit = querySettings.CreateForExtendedProperty<Territory, long>("restrictToOrganizationUnit",
                organizationUnit =>
                    {
                        var request = new SelectOrganizationUnitTerritoriesRequest { OrganizationUnitId = organizationUnit };
                        var territories = ((SelectOrganizationUnitTerritoriesResponse)this._publicService.Handle(request)).TerritoryIds;

                        return x => territories.Contains(x.Id);
                    });

            return query
                .Filter(_filterHelper, restrictToCurrentUserFilter, restrictToOrganizationUnit)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListTerritoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    OrganizationUnitId = x.OrganizationUnitId,
                    OrganizationUnitName = x.OrganizationUnit.Name,
                    IsActive = x.IsActive
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}