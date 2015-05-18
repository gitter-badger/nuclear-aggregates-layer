using System;
using System.Runtime.Serialization;

using NuClear.Model.Common;

namespace DoubleGis.Erm.Platform.Model.Identities.Properties
{
    [DataContract]
    public abstract class EntityPropertyIdentityBase<TConcreteIdentity> : IdentityBase<TConcreteIdentity>, IEntityPropertyIdentity
        where TConcreteIdentity : IdentityBase<TConcreteIdentity>, new()
    {
        public abstract string PropertyName { get; }
        public abstract Type PropertyType { get; }

        bool IEquatable<IEntityPropertyIdentity>.Equals(IEntityPropertyIdentity other)
        {
            return Equals(other);
        }
    }
}