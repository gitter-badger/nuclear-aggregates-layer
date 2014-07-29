using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.UploadBinary
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.UploadBinary201307)]
    public interface IUploadBinaryApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "POST", UriTemplate = "/{entityName}/{entityId}/{binaryId}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(UploadBinaryOperationErrorDescription))]
        UploadFileResult Execute(string entityName, string entityId, string binaryId, Stream multipartStream);
    }
}