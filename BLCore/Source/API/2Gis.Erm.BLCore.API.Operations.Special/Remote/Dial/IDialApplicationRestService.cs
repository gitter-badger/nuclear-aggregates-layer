using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.Dialing.Dial201503)]
    public interface IDialApplicationRestService
    {
        [OperationContract(Name = "DialRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/dial/{phone}")]
        [FaultContract(typeof(DialErrorDescription))]
        DialResult Dial(string phone);
    }
}
