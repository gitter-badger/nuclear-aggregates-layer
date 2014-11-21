using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;
using DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell.AvalonDockUtils;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Documents
{
    public sealed class DocumentManager : ViewModelBase, IDocumentManager, IDocumentManagerViewModel
    {
        private IEnumerable<IDocument> _documents;
        private IDocument _activeDocument;
        private readonly IDictionary<IDocument, ICollection<IDialog>> _dialogs = new Dictionary<IDocument, ICollection<IDialog>>(); 

        public DocumentManager(ILayoutComponentsRegistry componentsRegistry)
        {
            MainViewSelector =
                new ComponentSelector<ILayoutDocumentsComponent, IDocument>(componentsRegistry.GetComponentsForLayoutRegion<ILayoutDocumentsComponent>());
            HeaderViewSelector =
                new ComponentSelector<ILayoutDocumentHeadersComponent, IDocument>(componentsRegistry.GetComponentsForLayoutRegion<ILayoutDocumentHeadersComponent>());
            DialogsViewSelector =
                new ComponentSelector<ILayoutDialogComponent, IDialog>(componentsRegistry.GetComponentsForLayoutRegion<ILayoutDialogComponent>());
        }

        /// <summary>
        /// Свойство Documents использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public IEnumerable<IDocument> Documents
        {
            get
            {
                return _documents;
            }

            private set
            {
                    _documents = value;
                    RaisePropertyChanged(() => Documents);
                }
            }

        /// <summary>
        /// Свойство ActiveDocument использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public IDocument ActiveDocument
        {
            get
            {
                return _activeDocument;
            }

            set
            {
                var oldValue = _activeDocument;
                if (oldValue != value)
                {
                    if (value is NotDocumentMarker)
                    {
                        return;
                    }

                    Add2DocumentsHistory(value);
                    ChangeActiveDocument(value, oldValue);
                }
            }
        }

        public DataTemplateSelector MainViewSelector { get; private set; }
        public DataTemplateSelector HeaderViewSelector { get; private set; }
        public DataTemplateSelector DialogsViewSelector { get; private set; }

        public IEnumerable<IDialog> ActiveDialogs
        {
            // TODO: Equatable?
            get
            {
                var emptyResult = new IDialog[0];
                var activeDocument = ActiveDocument;
                var snapshot = _dialogs.ToArray();
                if (!snapshot.Any())
                {
                    return emptyResult;
                }

                var appropriateDialogs = snapshot.SingleOrDefault(pair => pair.Key.Equals(activeDocument)).Value;
                return appropriateDialogs;
            }
        }

        IEnumerable<IDocument> IDocumentsStateInfo.Documents
        {
            get
            {
                return Documents;
            }
        }

        IDocument IDocumentsStateInfo.ActiveDocument 
        {
            get
            {
                return ActiveDocument;
            }
        }

        bool IDocumentManager.TryActivate(IDocument document)
        {
            var currentActiveDocument = _activeDocument;
            if (currentActiveDocument == document)
            {
                return false;
            }

            var currentDocuments = Documents != null ? Documents.ToArray() : new IDocument[0];
            if (!currentDocuments.Contains(document))
            {
                return false;
            }

            Add2DocumentsHistory(document);
            ChangeActiveDocument(document, currentActiveDocument);

            return true;
        }

        void IDocumentManager.Add(IDocument document)
        {
            var addedElement = new[] { document };
            //_guiDispatcher.Invoke(()=>
            Add(addedElement)
            // )
            ;
        }

        void IDocumentManager.Add(IEnumerable<IDocument> documents)
        {
            Add(documents);
        }

        public void AddDialog(IDocument document, IDialog dialog)
        {
            ICollection<IDialog> dialogsForDocumnet;
            if (_dialogs.TryGetValue(document, out dialogsForDocumnet))
            {
                Application.Current.Dispatcher.Invoke(() => dialogsForDocumnet.Add(dialog));
            }
            else
            {
                _dialogs.Add(document, new ObservableCollection<IDialog> { dialog });
            }

            RaisePropertyChanged(() => ActiveDialogs);
        }

        void IDocumentManager.Remove(IDocument document)
        {
            var removedElement = new[] { document };
            Remove(removedElement);
        }

        void IDocumentManager.Remove(IEnumerable<IDocument> documents)
        {
            Remove(documents);
        }

        public void RemoveDialog(IDocument document, IDialog dialog)
        {
            ICollection<IDialog> dialogsForDocumnet;
            if (_dialogs.TryGetValue(document, out dialogsForDocumnet))
            {
                Application.Current.Dispatcher.Invoke(() => dialogsForDocumnet.Remove(dialog));
                RaisePropertyChanged(() => ActiveDialogs);
            }
        }

        public event Action<IDocument> ActiveDocumentChanged;

        private const int MaxActiveDocumentsHistoryLenght = 10;
        private IDocument[] _activeDocumentsHistory = new IDocument[0];

        private void ChangeActiveDocument(IDocument nextActiveDocument, IDocument previousActiveDocument)
        {
            _activeDocument = nextActiveDocument;

            bool oldActivityValue;
            TryToggleDocumentActive(previousActiveDocument, false, out oldActivityValue);
            TryToggleDocumentActive(nextActiveDocument, true, out oldActivityValue);

            RaisePropertyChanged(() => ActiveDocument);
            RaisePropertyChanged(() => ActiveDialogs);
            var activeDocumentChangedHandler = ActiveDocumentChanged;
            if (activeDocumentChangedHandler != null)
            {
                activeDocumentChangedHandler(nextActiveDocument);
            }
        }

        private void TryToggleDocumentActive(IDocument document, bool targetActivityValue, out bool previousActivityValue)
        {
            previousActivityValue = false;

            var activableDocument = document as IActivableDocument;
            if (activableDocument == null)
            {
                return;
            }

            previousActivityValue = activableDocument.IsActive;
            if (previousActivityValue == targetActivityValue)
            {
                return;
            }

            activableDocument.IsActive = targetActivityValue;
        }

        private void Add(IEnumerable<IDocument> documents)
        {
            var currentActiveDocument = _activeDocument;
            var addedDocuments = documents.ToArray();
            var currentDocuments = Documents != null ? Documents.ToArray() : new IDocument[0];
            
            var nextActiveDocument = addedDocuments.First();
            Add2DocumentsHistory(nextActiveDocument);

            var mergedDocument = currentDocuments.Length > 0 ? currentDocuments.Union(addedDocuments) : addedDocuments;
            Documents = mergedDocument.OrderByDescending(document => document is IContextualDocument);
            ChangeActiveDocument(nextActiveDocument, currentActiveDocument);
        }

        private void Add2DocumentsHistory(IDocument newActiveDocument)
        {
            var currentHistory = _activeDocumentsHistory;
            var activeDocumentsHistory = currentHistory != null ? new List<IDocument>(currentHistory) : new List<IDocument>();
            activeDocumentsHistory.Remove(newActiveDocument);
            if (activeDocumentsHistory.Count == MaxActiveDocumentsHistoryLenght)
            {
                activeDocumentsHistory.RemoveAt(MaxActiveDocumentsHistoryLenght - 1);
            }

            activeDocumentsHistory.Insert(0, newActiveDocument);
            _activeDocumentsHistory = activeDocumentsHistory.ToArray();
        }
        
        private void Remove(IEnumerable<IDocument> documents)
        {
            var currentActiveDocument = _activeDocument;
            var nextActiveDocument = currentActiveDocument;
            var removedDocuments = documents.ToArray();
            var currentDocuments = Documents != null ? Documents.ToArray() : new IDocument[0];
            var actualDocuments = currentDocuments.Except(removedDocuments).ToArray();
            var resultActiveDocumentHistory = RemoveFromDocumentsHistory(removedDocuments);

            if (nextActiveDocument != null && removedDocuments.Contains(nextActiveDocument))
            {   // смена активного документа
                nextActiveDocument = resultActiveDocumentHistory.FirstOrDefault() ?? actualDocuments.FirstOrDefault();
            }

            Documents = actualDocuments;
            if (currentActiveDocument != nextActiveDocument)
            {
                ChangeActiveDocument(nextActiveDocument, currentActiveDocument);
            }
        }

        private IEnumerable<IDocument> RemoveFromDocumentsHistory(IEnumerable<IDocument> removedDocuments)
        {
            var currentHistory = _activeDocumentsHistory;
            if (currentHistory == null)
            {
                _activeDocumentsHistory = new IDocument[0];
                return _activeDocumentsHistory;
            }

            var activeDocumentsHistory = new List<IDocument>(_activeDocumentsHistory);
            activeDocumentsHistory.RemoveAll(removedDocuments.Contains);
            _activeDocumentsHistory = activeDocumentsHistory.ToArray();
            return activeDocumentsHistory;
        }
    }
}