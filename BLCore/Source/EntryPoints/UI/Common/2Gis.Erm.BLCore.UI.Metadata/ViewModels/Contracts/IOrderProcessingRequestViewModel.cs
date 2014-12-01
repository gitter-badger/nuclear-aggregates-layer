using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOrderProcessingRequestViewModel : IEntityViewModelAbstract<OrderProcessingRequest>
    {
        string Title { get; set; }
    }
}