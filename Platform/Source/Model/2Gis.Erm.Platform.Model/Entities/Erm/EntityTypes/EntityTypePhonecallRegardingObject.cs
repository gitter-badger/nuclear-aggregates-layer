using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePhonecallRegardingObject : EntityTypeBase<EntityTypePhonecallRegardingObject>
    {
        public override int Id
        {
            get { return EntityTypeIds.PhonecallRegardingObject; }
        }

        public override string Description
        {
            get { return "PhonecallRegardingObject"; }
        }
    }
}