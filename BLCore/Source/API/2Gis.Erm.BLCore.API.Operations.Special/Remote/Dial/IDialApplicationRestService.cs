using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.Dialing.Dial201503)]
    public interface IDialApplicationRestService
    {
        [OperationContract(Name = "DialRest")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/dial/{phone}")]
        [FaultContract(typeof(DialErrorDescription))]
        void Dial(string phone);
    }
}
