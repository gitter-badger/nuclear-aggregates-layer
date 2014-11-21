using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.CheckForDebts
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.CheckForDebts201303)]
    public interface ICheckForDebtsApplicationRestService
    {
         [OperationContract(Name = "ExecuteRest")]
         [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityIds}", ResponseFormat = WebMessageFormat.Json)]
         [FaultContract(typeof(CheckForDebtsOperationErrorDescription))]
         CheckForDebtsResult Execute(string entityName, string entityIds);
    }
}