using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCommune : EntityTypeBase<EntityTypeCommune>
    {
        public override int Id
        {
            get { return EntityTypeIds.Commune; }
        }

        public override string Description
        {
            get { return "Commune"; }
        }
    }
}