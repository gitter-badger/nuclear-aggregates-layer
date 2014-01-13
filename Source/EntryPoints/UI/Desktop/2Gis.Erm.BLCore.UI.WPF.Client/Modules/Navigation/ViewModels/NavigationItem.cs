using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public sealed class NavigationItem : ViewModelBase, INavigationItem, IMessageSource<NavigationMessage>
    {
        private readonly ITitleProvider _titleProvider;
        private readonly DelegateCommand<INavigationItem> _navigationCommand;

        private readonly int _id;
        private bool _isExpanded;
        private bool _isSelected;

        public NavigationItem(int id, ITitleProvider titleProvider, DelegateCommand<INavigationItem> navigationCommand)
        {
            _id = id;
            _titleProvider = titleProvider;
            _navigationCommand = navigationCommand;
        }

        public string Title 
        {
            get
            {
                return _titleProvider.Title;
            }
        }

        public IImageProvider Icon { get; set; }

        public IDelegateCommand NavigateCommand 
        {
            get
            {
                return _navigationCommand;
            }
        }

        public INavigationItem[] Items { get; set; }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    RaisePropertyChanged(() => IsExpanded);
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected != value)
                {
                    if (value)
                    {
                        if (!_navigationCommand.CanExecute(this))
                        {
                            RaisePropertyChanged(() => IsSelected);
                            return;
                        }

                        _navigationCommand.Execute(this);
                    }

                    _isSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                }
            }
        }
    }
}