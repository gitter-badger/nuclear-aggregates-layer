using System;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IServiceAvailabilityProvider
    {
        EndpointDescription[] GetAvailableServices();
        EndpointDescription[] GetCompatibleServices(Version clientVersion);
    }
}
