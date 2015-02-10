using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeActivityStatus
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.ChangeActivityStatus201502)]
    public interface IChangeActivityStatusApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{status}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ChangeActivityStatusOperationErrorDescription))]
        void Execute(string entityName, string entityId, string status);
    }
}
