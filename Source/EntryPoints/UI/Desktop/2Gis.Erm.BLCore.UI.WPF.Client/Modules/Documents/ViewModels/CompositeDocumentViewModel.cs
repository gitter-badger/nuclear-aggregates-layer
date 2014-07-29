using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels
{
    public sealed class CompositeDocumentViewModel : ViewModelBase, ICompositeDocumentViewModel
    {
        private readonly IViewModel[] _composedViewModels;
        private readonly ITitleProvider _titleProvider;
        private readonly DataTemplateSelector _viewSelector;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        private readonly IContextualNavigationViewModel _contextualNavigationSource;

        public CompositeDocumentViewModel(ITitleProvider titleProvider, IEnumerable<IViewModel> composedViewModels, DataTemplateSelector viewSelector)
        {
            _composedViewModels = composedViewModels.ToArray();
            _titleProvider = titleProvider;
            _viewSelector = viewSelector;

            foreach (var composedViewModel in ComposedViewModels)
            {
                var inpcViewModel = composedViewModel as INotifyPropertyChanged;
                if (inpcViewModel != null)
                {
                    inpcViewModel.PropertyChanged += OnComposedViewModelChanged;
                }
            }

            _contextualNavigationSource = (IContextualNavigationViewModel)_composedViewModels.FirstOrDefault(vm => vm is IContextualNavigationViewModel);
        }

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public string DocumentName
        {
            get
            {
                return _titleProvider.Title;
            }
        }

        Guid IDocument.Id
        {
            get
            {
                return _identity.Id;
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged(() => IsActive);
                }
            }
        }

        IEnumerable<INavigationItem> IActionsContainerDocument.Actions
        {
            get
            {
                return _composedViewModels.OfType<IActionsBoundViewModel>().SelectMany(vm => vm.Actions).ToArray();
            }
        }

        IReadOnlyDictionary<string, INavigationItem> IContextualNavigationViewModel.Parts
        {
            get
            {
                return _contextualNavigationSource != null ? _contextualNavigationSource.Parts : null;
            }
        }

        object IContextualNavigationViewModel.ReferencedItemContext
        {
            get
            {
                return _contextualNavigationSource != null ? _contextualNavigationSource.ReferencedItemContext : null;
            }

            set
            {
                if (_contextualNavigationSource != null && _contextualNavigationSource.ReferencedItemContext != value)
                {
                    _contextualNavigationSource.ReferencedItemContext = value;
                    RaisePropertyChanged();
                }
            }
        }

        DataTemplateSelector IContextualNavigationViewModel.ReferencedItemViewSelector
        {
            get
            {
                return _contextualNavigationSource != null ? _contextualNavigationSource.ReferencedItemViewSelector : null;
            }
        }

        INavigationItem[] IContextualNavigationViewModel.Items
        {
            get
            {
                return _composedViewModels.OfType<IContextualNavigationViewModel>().SelectMany(vm => vm.Items).ToArray();
            }
        }

        ITitleProvider IContextualNavigationViewModel.Title
        {
            get
            {
                return _titleProvider;
            }
        }

        INavigationItem[] IDynamicContextualNavigation.Items
        {
            get
            {
                return ((IContextualNavigationViewModel)this).Items;
            }
        }

        ITitleProvider IDynamicContextualNavigation.Title
        {
            get
            {
                return ((IContextualNavigationViewModel)this).Title;
            }
        }

        public bool IsEmptyDocument 
        {
            get
            {
                return !_composedViewModels.Any();
            }
        }
        
        public bool IsDegenerateComposition
        {
            get
            {
                return _composedViewModels.Length == 1;
            }
        }

        public DataTemplateSelector ComposedViewSelector 
        {
            get
            {
                return _viewSelector;
            }
        }

        public IViewModel[] ComposedViewModels
        {
            get
            {
                return _composedViewModels;
            }
        }

        private void OnComposedViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseAllPropertiesChanged();
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
                    foreach (var composedViewModel in ComposedViewModels)
                    {
                        var inpcViewModel = composedViewModel as INotifyPropertyChanged;
                        if (inpcViewModel != null)
                        {
                            inpcViewModel.PropertyChanged -= OnComposedViewModelChanged;
                        }
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
