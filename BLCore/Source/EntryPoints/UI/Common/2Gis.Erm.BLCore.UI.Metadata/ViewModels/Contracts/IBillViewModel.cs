using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IBillViewModel : IEntityViewModelAbstract<Bill>
    {
        string BillNumber { get; set; }
    }
}
