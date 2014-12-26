using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOrderFileViewModel : IEntityViewModelAbstract<OrderFile>
    {
        string FileName { get; set; }
        bool UserDoesntHaveRightsToEditOrder { get; set; }
    }
}
