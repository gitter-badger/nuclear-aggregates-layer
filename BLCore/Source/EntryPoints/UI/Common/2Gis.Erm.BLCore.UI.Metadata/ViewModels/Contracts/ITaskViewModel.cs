using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ITaskViewModel : IEntityViewModelAbstract<Task>, IActivityViewModel
    {
        string Title { get; set; }
    }
}
