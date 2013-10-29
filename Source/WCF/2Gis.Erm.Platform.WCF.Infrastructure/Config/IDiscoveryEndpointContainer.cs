using System.Collections.Generic;
using System.ServiceModel.Discovery;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Config
{
    public interface IDiscoveryEndpointContainer
    {
        IEnumerable<DiscoveryEndpoint> Endpoints { get; }
    }
}