using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ICurrencyViewModel : IEntityViewModelAbstract<Currency>
    {
        string Name { get; set; }
        bool IsBase { get; set; }
    }
}