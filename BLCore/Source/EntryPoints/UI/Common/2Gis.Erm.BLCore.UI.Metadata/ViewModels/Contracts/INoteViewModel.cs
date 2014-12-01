using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface INoteViewModel : IEntityViewModelAbstract<Note>
    {
        string Title { get; set; }
    }
}