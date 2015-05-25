using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Navigation;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels
{
    public sealed class NullDocumentViewModel : ViewModelBase, IViewModel, IDocument, IDynamicContextualNavigation, IActivableDocument, IActionsContainerDocument
    {
        private readonly int _documentNumber;
        private INavigationItem _selectedDocumentPart;
        private bool _isActive;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();

        private readonly INavigationItem[] _items;
        private readonly ITitleProvider _title;

        public NullDocumentViewModel(int documentNumber)
        {
            _documentNumber = documentNumber;
            var items = new INavigationItem[]
                                {
                                    new FakeNavigationItem("Документ_" + documentNumber + "_Сведения")
                                        {
                                            Items = new INavigationItem[]
                                                        {
                                                            new FakeNavigationItem("Документ_" + documentNumber + "_Основные"),
                                                            new FakeNavigationItem("Документ_" + documentNumber + "_Дополнительно"),
                                                            new FakeNavigationItem("Документ_" + documentNumber + "_Администрирование"), 
                                                            new FakeNavigationItem("Документ_" + documentNumber + "_Примечание"), 
                                                            new FakeNavigationItem("Документ_" + documentNumber + "_История изменений")
                                                        }
                                        },
                                    new FakeNavigationItem("Документ_" + documentNumber + "_Счета на оплату"), 
                                    new FakeNavigationItem("Документ_" + documentNumber + "_Блокировки"),
                                    new FakeNavigationItem("Документ_" + documentNumber + "_Файлы к заказу")
                                };

            AttachNavigateCommand(items);
            _items = items;
            _title = new StaticTitleProvider(new StaticTitleDescriptor("Документ_" + documentNumber));
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
                return "Document_" + _documentNumber;
            }
        }

        Guid IDocument.Id
        {
            get
            {
                return _identity.Id;
            }
        }

        /// <summary>
        /// Свойство Prop использует интерфейс INotifyPropertyChanged 
        /// для уведомления View об изменениях во ViewModel
        /// </summary>
        public INavigationItem SelectedDocumentPart
        {
            get
            {
                return _selectedDocumentPart;
            }
            private set
            {
                if (_selectedDocumentPart != value)
                {
                    _selectedDocumentPart = value;
                    RaisePropertyChanged(() => SelectedDocumentPart);
                }
            }
        }

        public IEnumerable<INavigationItem> Actions { get; set; }

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
                    if (!value)
                    {
                        SelectedDocumentPart = null;
                    }

                    _isActive = value;
                }
            }
        }

        INavigationItem[] IDynamicContextualNavigation.Items
        {
            get
            {
                return _items;
            }
        }

        ITitleProvider IDynamicContextualNavigation.Title
        {
            get
            {
                return _title;
            }
        }

        private void AttachNavigateCommand(IEnumerable<INavigationItem> items)
        {
            foreach (var navigationItem in items)
            {
                if (navigationItem.Items != null && navigationItem.Items.Length > 0)
                {
                    AttachNavigateCommand(navigationItem.Items);
                }
                else
                {
                    var commandHost = navigationItem as FakeNavigationItem;
                    if (commandHost != null)
                    {
                        commandHost.NavigateCommand = new DelegateCommand<INavigationItem>(OnNavigateCommand);
                    }
                }
            }
        }

        private void OnNavigateCommand(INavigationItem navigationTarget)
        {
            SelectedDocumentPart = navigationTarget;
        }
    }
}
