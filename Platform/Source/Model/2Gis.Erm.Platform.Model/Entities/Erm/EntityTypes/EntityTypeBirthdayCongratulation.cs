using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBirthdayCongratulation : EntityTypeBase<EntityTypeBirthdayCongratulation>
    {
        public override int Id
        {
            get { return EntityTypeIds.BirthdayCongratulation; }
        }

        public override string Description
        {
            get { return "BirthdayCongratulation"; }
        }
    }
}