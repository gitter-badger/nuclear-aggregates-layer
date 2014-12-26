using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOrderViewModel : IEntityViewModelAbstract<Order>
    {
        string OrderNumber { get; set; }
        int WorkflowStepId { get; set; }
    }
}