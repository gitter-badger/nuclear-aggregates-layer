using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLetter : EntityTypeBase<EntityTypeLetter>
    {
        public override int Id
        {
            get { return EntityTypeIds.Letter; }
        }

        public override string Description
        {
            get { return "Letter"; }
        }
    }
}