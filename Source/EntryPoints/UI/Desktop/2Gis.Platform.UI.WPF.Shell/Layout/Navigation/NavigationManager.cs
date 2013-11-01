using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Navigation
{
    public sealed class NavigationManager : ViewModelBase, INavigationManager, INavigationManagerViewModel, IDisposable
    {
        private readonly IDocumentManager _documentManager;
        public NavigationManager(
            ILayoutComponentsRegistry componentsRegistry,
            IDocumentManager documentManager,
            INavigationAreasRegistry areasRegistry)
        {
            _documentManager = documentManager;
            
            ViewSelector = 
                new ComponentSelector<ILayoutNavigationComponent, INavigationArea>(componentsRegistry.GetComponentsForLayoutRegion<ILayoutNavigationComponent>());
            
            Areas = areasRegistry.AllAreas.ToArray();
            InitialNavigate();
            
            _documentManager.ActiveDocumentChanged += SynchronizeWithActiveDocument;
        }

        private void SynchronizeWithActiveDocument(IDocument currentActiveDocument)
        {
            var contextualArea = ContextualNavigationArea;
            if (contextualArea == null)
            {
                return;
            }

            var currentSelectedArea = _selectedArea;
            var previousSelectedArea = _previousSelectedArea;
            INavigationArea nextSelectedArea = currentSelectedArea;

            var dynamicContextualNavigationDocument = currentActiveDocument as IDynamicContextualNavigation;
            if (dynamicContextualNavigationDocument != null)
            {
                nextSelectedArea = contextualArea;
                contextualArea.UpdateContext(dynamicContextualNavigationDocument.Title, dynamicContextualNavigationDocument.Items);
            }
            else
            {
                var isNextDocumentContextual = currentActiveDocument is IContextualDocument;
                var isCurrentNavigationAreaContextual = currentSelectedArea is IContextualNavigationArea;

                if (isNextDocumentContextual && isCurrentNavigationAreaContextual)
                {
                    nextSelectedArea = previousSelectedArea ?? DefaultNonContextualArea;
                }
                else if (!isNextDocumentContextual && isCurrentNavigationAreaContextual)
                {
                    nextSelectedArea = previousSelectedArea ?? DefaultNonContextualArea;
                }
                else if (!isNextDocumentContextual)
                {
                    // do nothing
                }
                else
                {
                    // выбрали контекстный документ => пока никакой обработки не делаем, чаще всего сюда будем попадать из-за 
                    // смены selected navigationarea => trychangeactivedocument ранее по поптоку выполнения (выше по стеку вызовов)
                    // do nothing
                }
            }

            if (currentSelectedArea != nextSelectedArea)
            {
                SwitchSelectedArea(nextSelectedArea, _selectedArea);
            }
        }

        private void SwitchSelectedArea(
            INavigationArea nextSelectedArea, 
            INavigationArea currentSelectedArea)
        {
            if (currentSelectedArea == nextSelectedArea)
            {
                return;
            }

            if (currentSelectedArea != null)
            {
                currentSelectedArea.IsSelected = false;
                _previousSelectedArea = currentSelectedArea;
            }

            _selectedArea = nextSelectedArea;
            if (nextSelectedArea != null)
            {
                nextSelectedArea.IsSelected = true;

                if (!(nextSelectedArea is IContextualNavigationArea))
                {
                    TryActivateContextualDocument();
                }
            }

            RaisePropertyChanged(() => SelectedArea);
        }

        private void TryActivateContextualDocument()
        {
            var contextualDocument = _documentManager.Documents.SingleOrDefault(document => document is IContextualDocument);
            if (contextualDocument != null)
            {
                _documentManager.TryActivate(contextualDocument);
            }
        }

        private void InitialNavigate()
        {
            var defaultNonContextualArea = DefaultNonContextualArea;
            if (defaultNonContextualArea != null)
            {
                SelectedArea = defaultNonContextualArea;
            }
        }

        private INavigationArea DefaultNonContextualArea 
        {
            get
            {
                var areas = _areas;
                return areas != null ? areas.FirstOrDefault(a => !(a is IContextualNavigationArea)) : null; 
            }
        }

        private IContextualNavigationArea ContextualNavigationArea
        {
            get
            {
                var areas = _areas;
                return (IContextualNavigationArea)(areas != null ? areas.SingleOrDefault(a => a is IContextualNavigationArea) : null);
            }
        }

        private IEnumerable<INavigationArea> _areas;
        /// <summary>
        /// Свойство Areas использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public IEnumerable<INavigationArea> Areas
        {
            get
            {
                return _areas;
            }
            private set
            {
                _areas = value;
                RaisePropertyChanged(() => Areas);
            }
        }

        private INavigationArea _previousSelectedArea;

        private INavigationArea _selectedArea;
        /// <summary>
        /// Свойство ActiveArea использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public INavigationArea SelectedArea
        {
            get
            {
                return _selectedArea;
            }
            set
            {
                SwitchSelectedArea(value, _selectedArea);
            }
        }

        public DataTemplateSelector ViewSelector { get; private set; }

        IEnumerable<INavigationArea> INavigationManager.Areas
        {
            get
            {
                return Areas;
            }
        }

        INavigationArea INavigationManager.SelectedArea 
        {
            get
            {
                return SelectedArea;
            }
        }

        #region Поддержка IDisposable

        private readonly Object _disposeSync = new Object();
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
                    _documentManager.ActiveDocumentChanged -= SynchronizeWithActiveDocument;
                    Areas = null;
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