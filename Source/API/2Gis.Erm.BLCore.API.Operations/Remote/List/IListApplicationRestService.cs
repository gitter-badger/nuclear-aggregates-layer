using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.List
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed, Namespace = ServiceNamespaces.BasicOperations.List201303)]
    public interface IListApplicationRestService
    {
        [OperationContract(Name = "ExecuteRest")]
        [WebInvoke(Method = "GET", UriTemplate = "/{entityName}?whereExp={whereExp}&start={start}&filterInput={filterInput}&extendedInfo={extendedInfo}&nameLocaleResourceId={nameLocaleResourceId}&limit={limit}&dir={dir}&sort={sort}&pId={parentId}&pType={parentType}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        [FaultContract(typeof(ListOperationErrorDescription))]
        ListResult Execute(string entityName,
                           string whereExp,
                           int start,
                           string filterInput,
                           string extendedInfo,
                           string nameLocaleResourceId,
                           int limit,
                           string dir,
                           string sort,
                           string parentId,
                           string parentType);
    }
}