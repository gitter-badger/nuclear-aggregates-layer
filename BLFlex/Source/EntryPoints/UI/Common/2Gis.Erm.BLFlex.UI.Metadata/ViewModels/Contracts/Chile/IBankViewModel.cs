using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.UI.Metadata.ViewModels.Contracts.Chile
{
    public interface IBankViewModel : IEntityViewModelAbstract<Bank>
    {
        string Name { get; set; }
    }
}
