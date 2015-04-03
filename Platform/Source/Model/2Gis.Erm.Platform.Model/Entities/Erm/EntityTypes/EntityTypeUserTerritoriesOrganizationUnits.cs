using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUserTerritoriesOrganizationUnits : EntityTypeBase<EntityTypeUserTerritoriesOrganizationUnits>
    {
        public override int Id
        {
            get { return EntityTypeIds.UserTerritoriesOrganizationUnits; }
        }

        public override string Description
        {
            get { return "UserTerritoriesOrganizationUnits"; }
        }
    }
}