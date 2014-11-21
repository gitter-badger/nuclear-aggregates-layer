using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public interface IViewModelWithFeedback : IViewModel
    {
        void ActivateNotificationSource(INotification entry);
    }
}