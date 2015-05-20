using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLetterSender : EntityTypeBase<EntityTypeLetterSender>
    {
        public override int Id
        {
            get { return EntityTypeIds.LetterSender; }
        }

        public override string Description
        {
            get { return "LetterSender"; }
        }
    }
}