using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBargainFile : EntityTypeBase<EntityTypeBargainFile>
    {
        public override int Id
        {
            get { return EntityTypeIds.BargainFile; }
        }

        public override string Description
        {
            get { return "BargainFile"; }
        }
    }
}