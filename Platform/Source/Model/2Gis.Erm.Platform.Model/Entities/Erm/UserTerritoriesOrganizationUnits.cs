using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class UserTerritoriesOrganizationUnits :
        IEntity
    {
        public long UserId { get; set; }
        public long OrganizationUnitId { get; set; }
        public long? TerritoryId { get; set; }

        public OrganizationUnit OrganizationUnit { get; set; }
        public Territory Territory { get; set; }
    }
}