using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListTerritoryService : ListEntityDtoServiceBase<Territory, ListTerritoryDto>
    {
        private readonly IPublicService _publicService;

        public ListTerritoryService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IPublicService publicService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _publicService = publicService;
        }

        protected override IEnumerable<ListTerritoryDto> GetListData(IQueryable<Territory> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var restrictToCurrentUserFilter = filterManager
                .CreateByParentEntity<Territory>(EntityName.User,
                                                 () =>
                                                     {
                                                         var currentUserTerritories = (SelectCurrentUserTerritoriesResponse)
                                                                                      _publicService.Handle(new SelectCurrentUserTerritoriesRequest());
                                                         return x => currentUserTerritories.TerritoryIds.Contains(x.Id);
                                                     });

            var restrictToOrganizationUnit = filterManager.CreateForExtendedProperty<Territory, long?>("restrictToOrganizationUnit",
                organizationUnit =>
                    {
                        var territories = default(IEnumerable<long>);
                        if (organizationUnit.HasValue)
                        {
                            var request = new SelectOrganizationUnitTerritoriesRequest { OrganizationUnitId = organizationUnit.Value };
                            territories = ((SelectOrganizationUnitTerritoriesResponse)_publicService.Handle(request)).TerritoryIds;
                        }

                        return x => organizationUnit.HasValue && territories.Contains(x.Id);
                    }, 
                    x => false);

            return query
                .ApplyFilter(restrictToCurrentUserFilter)
                .ApplyFilter(restrictToOrganizationUnit)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x =>
                        new ListTerritoryDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                OrganizationUnitId = x.OrganizationUnitId,
                                OrganizationUnitName = x.OrganizationUnit.Name,
                                IsActive = x.IsActive
                            });
        }
    }
}