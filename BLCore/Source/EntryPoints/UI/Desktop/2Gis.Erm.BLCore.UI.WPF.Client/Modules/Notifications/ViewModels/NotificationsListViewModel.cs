using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Input;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging.Concrete;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.ViewModels
{
    public sealed class NotificationsListViewModel : ViewModelBase, INotificationList, INotificationsManager, IDisposable
    {
        private readonly ITitleProvider _contextualNotificationsTitle;
        private readonly ITitleProvider _systemNotificationsTitle;
        private readonly ITitleProvider _notificationDescriptionTitle;

        private IEnumerable<INotification> _contextualNotifications;
        private IEnumerable<INotification> _systemNotifications;

        private readonly IDocumentsStateInfo _documentsStateInfo;
        private readonly IMessageSink _messageSink;

        private readonly object _notificationsSync = new object();
        private readonly Dictionary<Guid, List<IContextualNotification>> _contextualNotificationsBySources = new Dictionary<Guid, List<IContextualNotification>>();
        private readonly Dictionary<Guid, List<ISystemNotification>> _systemNotificationsBySources = new Dictionary<Guid, List<ISystemNotification>>();

        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();

        private const int CleanupIntervalMs = 5000;
        private readonly Timer _cleanupTimer;

        public NotificationsListViewModel(IDocumentsStateInfo documentsStateInfo, IMessageSink messageSink, ITitleProviderFactory titleProviderFactory)
        {
            _documentsStateInfo = documentsStateInfo;
            _messageSink = messageSink;
            
            ActivateNotificationSourceCommand = new DelegateCommand<INotification>(ProvideFeedbackToSource);
            _documentsStateInfo.ActiveDocumentChanged += OnActiveDocumentChanged;

            _contextualNotificationsTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.NotificationsContextualTitle));
            _systemNotificationsTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.NotificationsSystemTitle));
            _notificationDescriptionTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.NotificationsDescriptionTitle));

            _cleanupTimer = new Timer{ AutoReset = false, Enabled = false, Interval = CleanupIntervalMs };
            _cleanupTimer.Elapsed += NotificationsCleanup;

            StartCleanup();
        }

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public string ContextualNotificationsTitle 
        {
            get
            {
                return _contextualNotificationsTitle.Title;
            }
        }

        public string SystemNotificationsTitle
        {
            get
            {
                return _systemNotificationsTitle.Title;
            }
        }

        public string NotificationDescriptionTitle
        {
            get
            {
                return _notificationDescriptionTitle.Title;
            }
        }

        public IEnumerable<INotification> ContextualNotifications
        {
            get
            {
                return _contextualNotifications;
            }
            private set
            {
                _contextualNotifications = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<INotification> SystemNotifications
        {
            get
            {
                return _systemNotifications;
            }
            private set
            {
                _systemNotifications = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ActivateNotificationSourceCommand { get; private set; }

        void INotificationsManager.SetNotifications(INotification[] entries)
        {
            var modifiedSources = new HashSet<Guid>();

            lock (_notificationsSync)
            {
                foreach (var notification in entries)
                {
                    switch (notification.NotificationType)
                    {
                        case NotificationType.System:
                        {
                            var systemNotification = (ISystemNotification)notification;
                            List<ISystemNotification> persistance;
                            if (!_systemNotificationsBySources.TryGetValue(systemNotification.SourceId, out persistance))
                            {
                                persistance = new List<ISystemNotification>();
                                _systemNotificationsBySources.Add(systemNotification.SourceId, persistance);
                            }

                            modifiedSources.Add(notification.SourceId);
                            persistance.Add(systemNotification);
                            break;
                        }
                        case NotificationType.Contextual:
                        {
                            var contextualNotification = (IContextualNotification)notification;
                            List<IContextualNotification> persistance;
                            if (!_contextualNotificationsBySources.TryGetValue(contextualNotification.SourceId, out persistance))
                            {
                                persistance = new List<IContextualNotification>();
                                _contextualNotificationsBySources.Add(contextualNotification.SourceId, persistance);
                            }
                            else if (!modifiedSources.Contains(contextualNotification.SourceId))
                            {
                                persistance.Clear();
                            }
                            
                            modifiedSources.Add(notification.SourceId);
                            persistance.Add(contextualNotification);
                            break;
                        }
                        default:
                        {
                            throw new NotSupportedException("Not supported notification type: " + notification.NotificationType);
                        }
                    }
                }
            }

            UpdateNotificationsSnapshots();
        }

        private void UpdateNotificationsSnapshots()
        {
            var activeDocument = _documentsStateInfo.ActiveDocument;

            var contextualNotificationsSnapshot = new List<INotification>();
            IEnumerable<INotification> systemNotificationsSnapshot;

            lock (_notificationsSync)
            {
                var modifiedContextualSourceForCurrentContext = _contextualNotificationsBySources.Keys.Where(activeDocument.ContainsElement);
                
                foreach (var source in modifiedContextualSourceForCurrentContext)
                {
                    List<IContextualNotification> notifications;
                    if (_contextualNotificationsBySources.TryGetValue(source, out notifications))
                    {
                        contextualNotificationsSnapshot.AddRange(notifications);
                    }
                }

                systemNotificationsSnapshot = _systemNotificationsBySources.SelectMany(pair => pair.Value).OrderByDescending(n=>n.TimestampUtc).ToArray();
            }

            ContextualNotifications = contextualNotificationsSnapshot.OrderByDescending(n => n.TimestampUtc).ToArray();
            SystemNotifications = systemNotificationsSnapshot;
        }

        private void ProvideFeedbackToSource(INotification entry)
        {
            if (entry == null)
            {
                return;
            }

            lock (_notificationsSync)
            {
                if (!_contextualNotificationsBySources.ContainsKey(entry.SourceId) && !_systemNotificationsBySources.ContainsKey(entry.SourceId))
                {
                    return;
                }
            }

            _messageSink.Post(new NotificationFeedbackMessage(entry.SourceId, entry));
        }

        private void OnActiveDocumentChanged(IDocument document)
        {
            UpdateNotificationsSnapshots();
        }

        private void NotificationsCleanup(object state, ElapsedEventArgs eventArgs)
        {
            var currentDocuments = _documentsStateInfo.Documents.ToArray();
            var expirationBoundary = DateTime.UtcNow;

            var modifiedSources = new HashSet<Guid>();

            lock (_notificationsSync)
            {
                var contextualNotificationsBySourcesSnapshot = _contextualNotificationsBySources.ToArray();
                foreach (var contextualNotificationsBySource in contextualNotificationsBySourcesSnapshot)
                {
                    if (!currentDocuments.Any(i => i.ContainsElement(contextualNotificationsBySource.Key)))
                    {
                        _contextualNotificationsBySources.Remove(contextualNotificationsBySource.Key);
                        modifiedSources.Add(contextualNotificationsBySource.Key);
                    }
                }

                var systemNotificationsBySourcesSnapshot = _systemNotificationsBySources.ToArray();
                foreach (var systemNotificationsBySource in systemNotificationsBySourcesSnapshot)
                {
                    var notExpiredSystemNotifications =
                        systemNotificationsBySource.Value.Where(n => !n.ExpiredTimeUtc.HasValue || n.ExpiredTimeUtc > expirationBoundary).ToList();
                    if (!notExpiredSystemNotifications.Any())
                    {
                        _systemNotificationsBySources.Remove(systemNotificationsBySource.Key);
                        modifiedSources.Add(systemNotificationsBySource.Key);
                        continue;
                    }

                    if (notExpiredSystemNotifications.Count != systemNotificationsBySource.Value.Count)
                    {
                        _systemNotificationsBySources[systemNotificationsBySource.Key] = notExpiredSystemNotifications;
                        modifiedSources.Add(systemNotificationsBySource.Key);
                    }
                }
            }

            if (modifiedSources.Any())
            {
                UpdateNotificationsSnapshots();
            }

            if (!IsDisposed)
            {
                StartCleanup();
            }
        }

        private void StartCleanup()
        {
            var timer = _cleanupTimer;
            if (timer != null)
            {
                timer.Start();
            }
        }

        #region Поддержка IDisposable и Finalize

        private readonly object _disposeSync = new object();
        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    // Free other state (managed objects).
                    _documentsStateInfo.ActiveDocumentChanged -= OnActiveDocumentChanged;
                    var cleanupTimer = _cleanupTimer;
                    if (cleanupTimer != null)
                    {
                        cleanupTimer.Stop();
                        cleanupTimer.Elapsed -= NotificationsCleanup;
                        cleanupTimer.Dispose();
                    }
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }

        #endregion
    }
}