using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUkraineLegalPersonPart : EntityTypeBase<EntityTypeUkraineLegalPersonPart>
    {
        public override int Id
        {
            get { return EntityTypeIds.UkraineLegalPersonPart; }
        }

        public override string Description
        {
            get { return "UkraineLegalPersonPart"; }
        }
    }
}