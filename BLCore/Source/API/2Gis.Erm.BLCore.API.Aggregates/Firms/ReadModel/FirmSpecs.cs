using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public static partial class FirmSpecs
    {
        public static class Territories
        {
            public static class Find
            {
                public static FindSpecification<Territory> TerritoriesFromOrganizationUnits(IEnumerable<long> oranizationUnitIds)
                {
                    return new FindSpecification<Territory>(x => oranizationUnitIds.Contains(x.OrganizationUnitId));
                }

                public static FindSpecification<Territory> RegionalTerritories(string regionalTerritoryWord)
                {
                    return new FindSpecification<Territory>(x => x.IsActive && x.Name.Contains(regionalTerritoryWord));
                }
            }
        }
    }
}