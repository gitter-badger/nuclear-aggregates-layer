using System;
using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual
{
    public sealed class ContextualDocumentViewModel : ViewModelBase, IContextualDocumentViewModel
    {
        private readonly DataTemplateSelector _viewSelector;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        
        private IContextualDocumentContext _context;
        private string _documentName;

        public ContextualDocumentViewModel(DataTemplateSelector contextDocumentumentSelector)
        {
            _viewSelector = contextDocumentumentSelector;
            UpdateDocumentName(null);
        }

        public IViewModelIdentity Identity
        {
            get { return _identity; }
        }

        public string DocumentName
        {
            get { return _documentName; }
        }

        Guid IDocument.Id
        {
            get { return _identity.Id; }
        }

        public bool IsActive { get; set; }

        public IContextualDocumentContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
                UpdateDocumentName(value);

                RaisePropertyChanged(() => Context);
            }
        }

        public DataTemplateSelector ViewSelector
        {
            get { return _viewSelector; }
        }
        
        public IViewModel[] ComposedViewModels 
        {
            get
            {
                var defaultComposition = new IViewModel[0];
                var context = _context;
                if (context == null)
                {
                    return defaultComposition;
                }

                var wrappedContext = context.Context as IViewModel;
                return wrappedContext != null ? new[] { wrappedContext } : defaultComposition;
            }
        }
        
        private void UpdateDocumentName(IContextualDocumentContext newContext)
        {
            if (newContext == null)
            {
                _documentName = "Empty context";
            }
            else
            {
                _documentName = _context.Title ?? "Emtpty context title";
            }

            RaisePropertyChanged(() => DocumentName);
        }
    }
}
