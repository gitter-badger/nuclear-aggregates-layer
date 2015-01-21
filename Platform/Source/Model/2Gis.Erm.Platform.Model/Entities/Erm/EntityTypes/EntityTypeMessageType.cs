using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeMessageType : EntityTypeBase<EntityTypeMessageType>
    {
        public override int Id
        {
            get { return EntityTypeIds.MessageType; }
        }

        public override string Description
        {
            get { return "MessageType"; }
        }
    }
}