using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IBargainFileViewModel : IEntityViewModelAbstract<BargainFile>
    {
        string FileName { get; set; }
    }
}
