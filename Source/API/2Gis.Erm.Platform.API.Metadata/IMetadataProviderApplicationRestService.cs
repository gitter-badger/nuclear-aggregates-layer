using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Metadata;

namespace DoubleGis.Erm.Platform.API.Metadata
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.Metadata.Metadata201307)]
    public interface IMetadataProviderApplicationRestService
    {
        [OperationContract(Name = "GetAvailableServicesRest")]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAvailableServices", ResponseFormat = WebMessageFormat.Json)]
        EndpointDescription[] GetAvailableServices();

        [OperationContract(Name = "GetCompatibleServicesRest")]
        [WebInvoke(Method = "GET", UriTemplate = "/GetCompatibleServices/{clientVersion}", ResponseFormat = WebMessageFormat.Json)]
        EndpointDescription[] GetCompatibleServices(string clientVersion);
    }
}
