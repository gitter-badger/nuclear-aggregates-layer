using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IActivityViewModel : IViewModelAbstract
    {
        ActivityStatus Status { get; set; }
    }
}
