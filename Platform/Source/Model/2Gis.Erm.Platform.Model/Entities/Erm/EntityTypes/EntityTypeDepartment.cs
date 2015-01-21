using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDepartment : EntityTypeBase<EntityTypeDepartment>
    {
        public override int Id
        {
            get { return EntityTypeIds.Department; }
        }

        public override string Description
        {
            get { return "Department"; }
        }
    }
}