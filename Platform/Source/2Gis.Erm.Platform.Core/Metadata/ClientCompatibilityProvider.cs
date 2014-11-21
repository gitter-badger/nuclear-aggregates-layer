using System;

using DoubleGis.Erm.Platform.API.Core.Metadata;

namespace DoubleGis.Erm.Platform.Core.Metadata
{
    public sealed class ClientCompatibilityProvider : IClientCompatibilityProvider
    {
        public bool IsCompatible(Version clientVersion, Version serviceVersion, string serviceName)
        {
            return clientVersion.Major == serviceVersion.Major
                   && clientVersion.Minor == serviceVersion.Minor
                   && clientVersion.Build == serviceVersion.Build;
        }
    }
}
