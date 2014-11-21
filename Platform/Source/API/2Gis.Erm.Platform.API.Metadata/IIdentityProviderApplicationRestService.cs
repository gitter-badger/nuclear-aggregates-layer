using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.Platform.API.Metadata
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.Identity.Identity201303)]
    public interface IIdentityProviderApplicationRestService
    {
        [OperationContract]
        [WebGet(UriTemplate = "NewIdentity", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(MetadataOperationErrorDescription))]
        long NewIdentity();
    }
}
