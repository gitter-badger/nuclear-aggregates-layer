using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLegalPerson : EntityTypeBase<EntityTypeLegalPerson>
    {
        public override int Id
        {
            get { return EntityTypeIds.LegalPerson; }
        }

        public override string Description
        {
            get { return "LegalPerson"; }
        }
    }
}