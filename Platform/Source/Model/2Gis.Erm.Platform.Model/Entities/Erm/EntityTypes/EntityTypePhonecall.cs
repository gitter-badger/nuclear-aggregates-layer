using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePhonecall : EntityTypeBase<EntityTypePhonecall>
    {
        public override int Id
        {
            get { return EntityTypeIds.Phonecall; }
        }

        public override string Description
        {
            get { return "Phonecall"; }
        }
    }
}