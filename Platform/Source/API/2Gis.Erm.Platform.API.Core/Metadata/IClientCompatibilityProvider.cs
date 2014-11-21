using System;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IClientCompatibilityProvider
    {
        bool IsCompatible(Version clientVersion, Version serviceVersion, string serviceName);
    }
}
