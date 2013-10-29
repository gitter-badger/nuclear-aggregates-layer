using System.Data;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFEntityStateEntry
    {
        public EFEntityStateEntry(object entity, EntityState state, bool isRelationship)
        {
            IsRelationship = isRelationship;
            State = state;
            Entity = entity;
        }

        public EFEntityStateEntry(object entity, EntityState state) : this(entity, state, false)
        {
        }

        public object Entity { get; private set; }

        public EntityState State { get; private set; }

        public bool IsRelationship { get; private set; }
    }
}
