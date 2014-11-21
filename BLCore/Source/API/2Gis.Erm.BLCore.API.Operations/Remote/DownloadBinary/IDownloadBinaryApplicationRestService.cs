using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.DownloadBinary
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.DownloadBinary201307)]
    public interface IDownloadBinaryApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebGet(UriTemplate = "/{entityName}/{binaryId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(DownloadBinaryOperationErrorDescription))]
        Stream Execute(string entityName, string binaryId);
    }
}