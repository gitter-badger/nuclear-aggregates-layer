using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    // FIXME {d.ivanov, 11.03.2014}: Интерфейс должен быть повешен на OrderReferencesReadModel и перемещён в BLCore. 
    //                               Пока это не сделано, OrderReferencesReadModel создаётся на new - что не годится.
    public interface IOrderReferencesReadModel : IAggregateReadModel<Order>
    {
        bool TryGetReferences(EntityName parentEntityName,
                              long parentEntityId,
                              out EntityReference clientRef,
                              out EntityReference firmRef,
                              out EntityReference legalPersonRef,
                              out EntityReference destOrganizationUnitRef);
    }
}
