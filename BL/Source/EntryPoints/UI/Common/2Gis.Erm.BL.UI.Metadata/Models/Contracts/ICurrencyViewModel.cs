using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Metadata.Models.Contracts
{
    public interface ICurrencyViewModel : IEntityViewModelAbstract<Currency>
    {
        string Name { get; set; }
        bool IsBase { get; set; }
    }
}