using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLetterRecipient : EntityTypeBase<EntityTypeLetterRecipient>
    {
        public override int Id
        {
            get { return EntityTypeIds.LetterRecipient; }
        }

        public override string Description
        {
            get { return "LetterRecipient"; }
        }
    }
}