using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.Remote.List
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.List201303)]
    public interface IListApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ListOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.List201303)]
        ListResult Execute(EntityName entityName,
                           int start,
                           string filterInput,
                           string extendedInfo,
                           string nameLocaleResourceId,
                           int limit,
                           string sort,
                           long? parentId,
                           EntityName parentType);
    }
}