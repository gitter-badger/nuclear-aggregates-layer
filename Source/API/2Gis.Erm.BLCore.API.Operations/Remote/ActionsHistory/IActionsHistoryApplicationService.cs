using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory
{
    // TODO {d.ivanov, 05.03.2013}: по-хорошему надо бы во всех ServiceContract и DataContract указать xml Namespace
    // лучше всего вместе с годом и месяцем публикации, типа http://2gis.ru/erm/api/service1/2013/03
    // сейчас в результирующем wsdl много tempuri.org и datacontract.org
    // DONE {m.pashuk, 27.03.2013}: Согласен про ServiceContract полностью, с DataContract - посложнее.
    //                              -> Namespace в DataContract нужен только в тех, которые улетают клиенту через SOAP-endpoint, сейчас такие выделить может быть непросто
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
    public interface IActionsHistoryApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(ActionsHistoryOperationErrorDescription), Namespace = ServiceNamespaces.BasicOperations.ActionsHistory201303)]
        ActionsHistoryDto GetActionsHistory(EntityName entityName, long entityId);
    }
}