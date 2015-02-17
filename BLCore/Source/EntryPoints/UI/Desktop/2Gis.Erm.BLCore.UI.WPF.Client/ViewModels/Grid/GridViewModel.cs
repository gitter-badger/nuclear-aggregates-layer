using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.Elements.Aspects.Features.Resources.Titles;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid
{
    public sealed class GridViewModel : ViewModelBase, IGridViewModel<IGridViewModelIdentity>, IDisposable
    {
        private const int RowsPerPage = 40;

        private readonly IGridViewModelIdentity _identity;
        private readonly IMessageSink _messageSink;

        private readonly IPagerViewModel _pagerViewModel;
        private readonly IFilterViewModel _filterViewModel;
        private readonly IListSelectorViewModel _listSelectorViewModel;
        private readonly IToolbarViewModel _toolbarViewModel;

        private readonly ITitleProvider _cantGetListingMessageFormat;

        private readonly IListNonGenericEntityService _listService;
        private readonly IUserInfo _userInfo;
        private readonly ICommonLog _logger;

        // COMMENT {all, 25.06.2014}: ConcurrentBag может иметь опасные side effect - memory leak - в данном случае храним строки по этому не опасно, однако при рефаторинге - обращать внимание
        private readonly ConcurrentBag<string> _sortingSettingsPriority = new ConcurrentBag<string>();
        private readonly ConcurrentDictionary<string, SortingDescriptor> _sortingSettings = new ConcurrentDictionary<string, SortingDescriptor>();
        
        private readonly object _sync = new object();

        private readonly Stack<ListingRequest> _requestStack = new Stack<ListingRequest>();
        private readonly AutoResetEvent _listingRequestProcessorSignal = new AutoResetEvent(false);
        private readonly CancellationTokenSource _listingRequestProcessorCancellation;
        private readonly Task _listingRequestProcessor;

        private object[] _listingStorage;
        private bool _isBusy;
        private bool _isInitialized;

        private DataViewViewModel _currentView;
        private object[] _selectedItems;
        private DelegateCommand<SortingDescriptor> _sortCommand;
        private DelegateCommand<NavigationDescriptor> _navigateCommand;
        private DelegateCommand<object> _showEntryDetailCommand;

        public GridViewModel(
            IGridViewModelIdentity identity,
            DataViewViewModel defaultViewSettings,
            IMessageSink messageSink,
            IPagerViewModel pagerViewModel,
            IFilterViewModel filterViewModel,
            IListSelectorViewModel listSelectorViewModel,
            IToolbarViewModel toolbarViewModel,
            IListNonGenericEntityService listService,
            ITitleProviderFactory titleProviderFactory,
            IUserInfo userInfo,
            ICommonLog logger)
        {
            if (defaultViewSettings == null)
            {
                throw new ArgumentNullException("defaultViewSettings");
            }

            _identity = identity;
            _listService = listService;
            _userInfo = userInfo;
            _logger = logger;
            _messageSink = messageSink;

            _pagerViewModel = pagerViewModel;
            _filterViewModel = filterViewModel;
            _toolbarViewModel = toolbarViewModel;
            _listSelectorViewModel = listSelectorViewModel;

            _cantGetListingMessageFormat =
                titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.ListingCantGetResultForEntityList));

            if (_pagerViewModel.Enabled)
            {
                _pagerViewModel.TryChangingTargetPage += OnTryChangingTargetPage;
            }

            if (_filterViewModel.Enabled)
            {
                _filterViewModel.ApplingFilter += OnFilterApply;
            }

            _currentView = defaultViewSettings;
            if (_listSelectorViewModel.Enabled)
            {
                _listSelectorViewModel.SelectedView = _listSelectorViewModel.AvailableViews.Single(v => ReferenceEquals(v, defaultViewSettings));
                _listSelectorViewModel.SelectedViewChanged += OnSelectedViewChanged;
            }

            _listingRequestProcessorCancellation = new CancellationTokenSource();
            _listingRequestProcessor = Task.Factory.StartNew(ListingRequestProcessor, _listingRequestProcessorCancellation.Token);
        }

        public IPagerViewModel Pager
        {
            get { return _pagerViewModel; }
        }

        public IFilterViewModel Filter
        {
            get { return _filterViewModel; }
        }

        public IListSelectorViewModel ViewSelector
        {
            get { return _listSelectorViewModel; }
        }

        public IToolbarViewModel Toolbar
        {
            get { return _toolbarViewModel; }
        }

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public IGridViewModelIdentity ConcreteIdentity
        {
            get
            {
                return _identity;
            }
        }

        public object[] ListingStorage
        {
            get
            {
                bool isInitialized;
                object[] listingStorage;

                lock (_sync)
                {
                    isInitialized = _isInitialized;
                    listingStorage = _listingStorage;
                }

                if (!isInitialized)
                {
                    AddLisingRequest(CreateListingRequest());
                    return new object[0];
                }

                return listingStorage;
            }
        }
        
        public DataViewViewModel CurrentView
        {
            get
            {
                return _currentView;
            }
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public object[] SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                if (_selectedItems != value)
                {
                    _selectedItems = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                lock (_sync)
                {
                    return _isBusy;
                }
            }
            private set
            {
                lock (_sync)
                {
                    if (_isBusy.Equals(value))
                    {
                        return;
                    }

                    _isBusy = value;
                }
                
                RaisePropertyChanged();
            }
        }
        
        public DelegateCommand<NavigationDescriptor> NavigateCommand
        {
            get { return _navigateCommand ?? (_navigateCommand = new DelegateCommand<NavigationDescriptor>(OnNavigateCommand)); }
        }

        public DelegateCommand<object> ShowEntryDetailCommand
        {
            get { return _showEntryDetailCommand ?? (_showEntryDetailCommand = new DelegateCommand<object>(OnShowEntryDetailCommand)); }
        }

        public DelegateCommand<SortingDescriptor> SortCommand
        {
            get { return _sortCommand ?? (_sortCommand = new DelegateCommand<SortingDescriptor>(OnSortCommand)); }
        }
        
        private string EntityNameString 
        {
            get { return _identity.EntityName.ToStringLocalized(EnumResources.ResourceManager, _userInfo.Culture); }
        }
        
        private void OnNavigateCommand(NavigationDescriptor descriptor)
        {
            _messageSink.Post(new EntitySelectedMessage(descriptor.EntityName, descriptor.EntityId));
        }
        
        private void OnShowEntryDetailCommand(object targetEntry)
        {
            var entityId = Platform.UI.WPF.Infrastructure.ViewModel.ViewModelUtils.ExtractPropertyValue<long>(targetEntry, "Id");
            _messageSink.Post(new EntitySelectedMessage(_identity.EntityName, entityId));
        }

        private void OnSortCommand(SortingDescriptor sortingDescriptor)
        {
            _sortingSettingsPriority.Add(sortingDescriptor.Column);
            _sortingSettings.AddOrUpdate(sortingDescriptor.Column, sortingDescriptor, (s, descriptor) => sortingDescriptor);
        }

        private void OnSelectedViewChanged(DataViewViewModel selectedView)
        {
            CurrentView = selectedView;

            var request = CreateListingRequest();
            request.ViewSettings = selectedView;
            AddLisingRequest(request);
        }

        private void OnFilterApply(string filterText)
        {
            var request = CreateListingRequest();
            request.FilterText = filterText;
            AddLisingRequest(request);
        }

        private void OnTryChangingTargetPage(int targetPageNumber)
        {
            var request = CreateListingRequest();
            request.TargetPageNumber = targetPageNumber;
            AddLisingRequest(request);
        }

        private ListingRequest CreateListingRequest()
        {
            return new ListingRequest
            {
                TargetPageNumber = Pager.Enabled ? Pager.CurrentPageNumber : (int?)null,
                FilterText = Filter.Enabled ? Filter.FilterText : null,
                ViewSettings = CurrentView
            };
        }

        private void AddLisingRequest(ListingRequest requestRequest)
        {
            lock (_sync)
            {
                _requestStack.Push(requestRequest);
            }
            
            _listingRequestProcessorSignal.Set();
        }

        private void ListingRequestProcessor(object arg)
        {
            var cancellationToken = (CancellationToken)arg;

            ListingRequest currentProcessingRequest = null;
            CancellationTokenSource currentProcessingRequestCancellation = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                ListingRequest targetListingRequest = null;
                lock (_sync)
                {
                    while (_requestStack.Count > 0)
                    {
                        var request = _requestStack.Pop();
                        if (!request.RequireNew 
                            && currentProcessingRequest != null
                            && !IsNewProcessingRequired(request, currentProcessingRequest))
                        {
                            continue;
                        }

                        targetListingRequest = request;
                        currentProcessingRequest = targetListingRequest;
                        break;
                    }
                }

                if (targetListingRequest == null)
                {
                    _listingRequestProcessorSignal.WaitOne(5000);
                    continue;
                }

                var scopedCurrentProcessingRequestCancellation = currentProcessingRequestCancellation;
                if (scopedCurrentProcessingRequestCancellation != null)
                {
                    scopedCurrentProcessingRequestCancellation.Cancel(false);
                }

                scopedCurrentProcessingRequestCancellation = new CancellationTokenSource();
                currentProcessingRequestCancellation = scopedCurrentProcessingRequestCancellation;

                var executeListing = new Task<ListResult>(ExecuteListingRequest, targetListingRequest, scopedCurrentProcessingRequestCancellation.Token);
                executeListing.ContinueWith(task => FinishListingRequest(task, targetListingRequest, scopedCurrentProcessingRequestCancellation.Token), TaskContinuationOptions.NotOnCanceled);
                executeListing.Start();
            }
        }

        private ListResult ExecuteListingRequest(object state)
        {
            var listingRequest = (ListingRequest)state;

            IsBusy = true;

            // пока множественная сортировка не поддерживается
            var targetSorting = _sortingSettingsPriority.Select(column => _sortingSettings[column]).FirstOrDefault() ??
                                new SortingDescriptor { Column = _currentView.DefaultSortField, Direction = ListSortDirection.Descending };

            int limit = 0;
            int start = 0;
            if (Pager.Enabled)
            {
                limit = RowsPerPage;
                start = RowsPerPage * (GetTargetPageNumber(listingRequest) - 1);
            }

            string filterInput = string.Empty;
            if (Filter.Enabled && listingRequest != null)
            {
                filterInput = listingRequest.FilterText;
            }

            var searchListModel = new SearchListModel
            {
                Sort = targetSorting.Column,
                Start = start,
                Limit = limit,
                Dir = GetDirectionString(targetSorting.Direction),
                FilterInput = filterInput
            };

            return _listService.List(_identity.EntityName, searchListModel);
        }

        private int GetTargetPageNumber(ListingRequest listingRequest)
        {
            return listingRequest.TargetPageNumber.HasValue
                    && listingRequest.TargetPageNumber.Value > 0
                        ? listingRequest.TargetPageNumber.Value
                        : 1;
        }

        private void FinishListingRequest(Task<ListResult> listingTask, ListingRequest listingRequest, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            ListResult listResult = null;
            int resultTargetPageNumber = 0;

            try
            {
                listResult = listingTask.Result;
                resultTargetPageNumber = GetTargetPageNumber(listingRequest);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't get listing for entity " + _identity.EntityName);
                var msg = string.Format(_cantGetListingMessageFormat.Title, EntityNameString);
                _messageSink.Post(new NotificationMessage(new INotification[] { new SystemNotification(Guid.NewGuid(), NotificationLevel.Error, msg) }));
            }

            lock (_sync)
            {
                if (Pager.Enabled)
                {
                    Pager.UpdatePager(resultTargetPageNumber, listResult.RowCount / RowsPerPage);
                }

                var entriesStorage = ((IDataListResult)listResult).Data.Cast<object>().ToArray();
                _listingStorage = entriesStorage;
                _isInitialized = true;
                _isBusy = false;
            }

            RaisePropertyChanged(() => ListingStorage);
            RaisePropertyChanged(() => IsBusy);
        }

        private bool IsNewProcessingRequired(ListingRequest checkingRequest, ListingRequest currentRequest)
        {
            // пока не проверяем изменился ли запрос листинга, инициируем новый запрос всегда
            // однако, если впилить сравнение выполняемого запроса, а кандидата из очереди - можно не выполнять листинг, если реально запрос не изменился => 
            // запрос данных не приведет ни к чему кроме никому не нужной работы
            return true;
        }

        private static string GetDirectionString(ListSortDirection? direction)
        {
            if (direction == null)
            {
                return null;
            }

            return direction == ListSortDirection.Ascending ? "ASC" : "DESC";
        }

        #region Поддержка IDisposable

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
                    if (_pagerViewModel.Enabled)
                    {
                        _pagerViewModel.TryChangingTargetPage -= OnTryChangingTargetPage;
                    }

                    if (_filterViewModel.Enabled)
                    {
                        _filterViewModel.ApplingFilter -= OnFilterApply;
                    }

                    if (_listSelectorViewModel.Enabled)
                    {
                        _listSelectorViewModel.SelectedViewChanged -= OnSelectedViewChanged;
                    }

                    var listingRequestProcessorSignal = _listingRequestProcessorSignal;
                    if (listingRequestProcessorSignal != null)
                    {
                        var cts = _listingRequestProcessorCancellation;
                        if (cts != null)
                        {
                            cts.Cancel();
                        }

                        listingRequestProcessorSignal.Set();

                        var listingRequestProcessor = _listingRequestProcessor;
                        try
                        {
                            if (listingRequestProcessor != null)
                            {
                                listingRequestProcessor.Wait();
                            }
                        }
                        catch (Exception ex)
                        {
                            // do nothing
                        }

                        listingRequestProcessorSignal.Close();
                    }
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _isDisposed = true;
            }
        }

        #endregion
    }
}