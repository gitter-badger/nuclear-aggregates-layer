using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public sealed class FunctionalPrivilegeDepth :
        IEntity,
        IEntityKey
    {
        public long Id { get; set; }
        public long PrivilegeId { get; set; }
        public string LocalResourceName { get; set; }
        public byte Priority { get; set; }
        public int Mask { get; set; }

        public Privilege Privilege { get; set; }

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