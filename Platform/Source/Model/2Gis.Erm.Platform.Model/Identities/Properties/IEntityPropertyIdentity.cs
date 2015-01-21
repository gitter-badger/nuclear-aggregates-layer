using System;

using NuClear.Model.Common;

namespace DoubleGis.Erm.Platform.Model.Identities.Properties
{
    public interface IEntityPropertyIdentity : IIdentity, IEquatable<IEntityPropertyIdentity>
    {
        string PropertyName { get; }
        Type PropertyType { get; }
    }
}