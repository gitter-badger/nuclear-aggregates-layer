using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    [DataContract]
    public abstract class OperationIdentityBase<TConcreteIdentity> : IdentityBase<TConcreteIdentity>, IOperationIdentity
        where TConcreteIdentity : IdentityBase<TConcreteIdentity>, new()
    {
        bool IEquatable<IOperationIdentity>.Equals(IOperationIdentity other)
        {
            return Equals(other);
        }
    }
}