using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Notifications;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.ViewModels;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability
// ReSharper restore CheckNamespace
{
    public static partial class DesignTimeDataContainer
    {
        public static class Notifications
        {
            public static NotificationsListViewModel ViewModel
            {
                get
                {
                    var fakeDocument = new NullDocumentViewModel(0);
                    var documentsStateInfo = new NullDocumentsStateInfo(new IDocument[] { fakeDocument });
                    var notificationsListViewModel = new NotificationsListViewModel(documentsStateInfo, new NullMessageSink(), new TitleProviderFactory(new NullUserInfo("fakeuser")));
                    var notificationsManager = notificationsListViewModel as INotificationsManager;

                    notificationsManager.SetNotifications(FakeNotificationsProvider.GetContextualNotifications(fakeDocument.Identity.Id));
                    notificationsManager.SetNotifications(FakeNotificationsProvider.GetSystemNotifications(fakeDocument.Identity.Id));



                    return notificationsListViewModel;
                }
            }
        }
    }
}
