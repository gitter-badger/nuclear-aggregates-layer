using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePhonecallRecipient : EntityTypeBase<EntityTypePhonecallRecipient>
    {
        public override int Id
        {
            get { return EntityTypeIds.PhonecallRecipient; }
        }

        public override string Description
        {
            get { return "PhonecallRecipient"; }
        }
    }
}