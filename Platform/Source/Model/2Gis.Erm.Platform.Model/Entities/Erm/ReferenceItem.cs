using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class ReferenceItem :
        IEntity,
        IEntityKey,
        IDeletableEntity
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public long ReferenceId { get; set; }
        public bool IsDeleted { get; set; }

        public Reference Reference { get; set; }

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