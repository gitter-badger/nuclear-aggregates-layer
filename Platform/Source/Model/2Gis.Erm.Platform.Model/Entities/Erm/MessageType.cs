using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class MessageType :
        IEntity,
        IEntityKey
    {
        public MessageType()
        {
            LocalMessages = new HashSet<LocalMessage>();
        }

        public long Id { get; set; }
        public int IntegrationType { get; set; }
        public int SenderSystem { get; set; }
        public int ReceiverSystem { get; set; }

        public ICollection<LocalMessage> LocalMessages { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}