using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.List
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.List201303)]
    public interface IListApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ListOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.List201303)]
        ListResult Execute(EntityName entityName,
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