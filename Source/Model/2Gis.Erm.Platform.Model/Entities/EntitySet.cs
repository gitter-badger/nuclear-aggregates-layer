using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    [DataContract]
    public sealed class EntitySet : IEquatable<EntitySet>
    {
        public static class Create
        {
            public static EntitySet NonCoupled
            {
                get
                {
                    return new EntitySet(new[] { EntityName.None });
                }
            }

            public static EntitySet GenericEntitySpecific
            {
                get
                {
                    return new EntitySet(new[] { EntityName.All });
                }
            }
        }

        [DataMember]
        private readonly EntityName[] _entities;

        public EntitySet(params EntityName[] entities)
        {
            if (entities == null || entities.Length == 0)
            {
                throw new ArgumentException("Argument has invaid value");
            }

            _entities = entities;
        }

        public EntityName[] Entities
        {
            get
            {
                return _entities;
            }
        }

        #region Implementation of IEquatable<EntitiesDescriptor>

        bool IEquatable<EntitySet>.Equals(EntitySet other)
        {
            return Equals(other);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(EntitySet) != obj.GetType())
            {
                return false;
            }

            var other = (EntitySet)obj;

            if (ReferenceEquals(Entities, other.Entities))
            {
                return true;
            }

            if (Entities.Length != other.Entities.Length)
            {
                return false;
            }

            for (int i = 0; i < Entities.Length; i++)
            {
                if (Entities[i] != other.Entities[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Entities.EvaluateHashSimplified();
        }

        public override string ToString()
        {
            return Entities.EntitiesToString();
        }
    }
}