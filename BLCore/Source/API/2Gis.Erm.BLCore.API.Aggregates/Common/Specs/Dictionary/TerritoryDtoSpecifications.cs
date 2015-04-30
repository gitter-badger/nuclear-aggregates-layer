using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public class TerritoryDtoSpecifications
    {
        public static FindSpecification<TerritoryDto> TerritoriesFromOrganizationUnit(long oranizationUnitId)
        {
            return new FindSpecification<TerritoryDto>(x => x.OrganizationUnitId == oranizationUnitId);
        }
    }
}