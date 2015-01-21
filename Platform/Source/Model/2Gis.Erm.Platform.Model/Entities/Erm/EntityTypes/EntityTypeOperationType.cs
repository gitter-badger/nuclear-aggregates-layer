using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOperationType : EntityTypeBase<EntityTypeOperationType>
    {
        public override int Id
        {
            get { return EntityTypeIds.OperationType; }
        }

        public override string Description
        {
            get { return "OperationType"; }
        }
    }
}