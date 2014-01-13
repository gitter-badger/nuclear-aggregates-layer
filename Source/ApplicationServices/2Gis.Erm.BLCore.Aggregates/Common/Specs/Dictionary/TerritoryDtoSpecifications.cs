using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Dictionary
{
    public class TerritoryDtoSpecifications
    {
        public static FindSpecification<TerritoryDto> TerritoriesFromOrganizationUnit(long oranizationUnitId)
        {
            return new FindSpecification<TerritoryDto>(x => x.OrganizationUnitId == oranizationUnitId);
        }  
    }
}