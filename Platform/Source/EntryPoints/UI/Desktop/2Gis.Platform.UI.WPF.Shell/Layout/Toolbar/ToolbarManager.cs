using System;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Toolbar
{
    public sealed class ToolbarManager : IToolbarManagerViewModel, IToolbarManager, IDisposable
    {
        private readonly IToolbar _toolbar;
        private readonly IDocumentsStateInfo _documentStateInfo;

        public ToolbarManager(IToolbar toolbar, IDocumentsStateInfo documentStateInfo, ILayoutComponentsRegistry componentsRegistry)
        {
            _toolbar = toolbar;
            _documentStateInfo = documentStateInfo;

            _documentStateInfo.ActiveDocumentChanged += OnActiveDocumentChanged;

            ViewSelector =
                new ComponentSelector<ILayoutToolbarComponent, IToolbar>(componentsRegistry.GetComponentsForLayoutRegion<ILayoutToolbarComponent>());
        }

        public DataTemplateSelector ViewSelector { get; private set; }

        public IToolbar Toolbar
        {
            get { return _toolbar; }
        }

        public void InvalidateActionsAvailability()
        {
            if (_toolbar.Items == null)
            {
                return;
            }

            foreach (var item in _toolbar.Items)
            {
                if (item.NavigateCommand != null)
                {
                    item.NavigateCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        private void OnActiveDocumentChanged(IDocument nextActiveDocument)
        {
            var actionsContainer = nextActiveDocument as IActionsContainerDocument;
            _toolbar.Items = actionsContainer != null ? actionsContainer.Actions : null;
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
                    // Free other state (managed objects).
                    _documentStateInfo.ActiveDocumentChanged -= OnActiveDocumentChanged;
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
