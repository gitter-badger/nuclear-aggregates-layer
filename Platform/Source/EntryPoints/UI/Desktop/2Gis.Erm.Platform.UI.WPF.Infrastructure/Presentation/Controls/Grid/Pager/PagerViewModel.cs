using System;

using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager
{
    public sealed class PagerViewModel : ViewModelBase, IPagerViewModel
    {
        private readonly ITitleProvider _beforePageText;
        private readonly ITitleProvider _afterPageText;
        private readonly ITitleProvider _firstPageText;
        private readonly ITitleProvider _lastPageText;
        private readonly ITitleProvider _nextPageText;
        private readonly ITitleProvider _prevPageText;

        public PagerViewModel(ITitleProviderFactory titleProviderFactory)
        {
            _totalPagesCount = 0;
            _currentPageNumber = 0;

            _beforePageText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.BeforePageText));
            _afterPageText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.AfterPageText));
            _firstPageText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.FirstPageText));
            _lastPageText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.LastPageText));
            _nextPageText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.NextPageText));
            _prevPageText = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.PrevPageText));
        }

        public bool Enabled
        {
            get { return true; }
        }

        public event Action<int> TryChangingTargetPage;

        public void UpdatePager(int currentPageNumber, int totalPagesCount)
        {
            _currentPageNumber = currentPageNumber;
            _totalPagesCount = totalPagesCount;
            RaisePropertyChanged(() => CurrentPageNumber);
            RaisePropertyChanged(() => TotalPagesCount);
            RaiseCommandsCanExecuteChanged();
        }

        private int _totalPagesCount;
        public int TotalPagesCount
        {
            get { return _totalPagesCount; }
        }

        private int _currentPageNumber;
        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
        }

        private void FirePagerStateChanged(int targetPage)
        {
            var handler = TryChangingTargetPage;
            if (handler != null)
            {
                handler(targetPage);
            }
        }

        private DelegateCommand _nextCommand;
        public DelegateCommand NextCommand
        {
            get { return _nextCommand ?? (_nextCommand = new DelegateCommand(OnNextCommand, CanExecuteNextCommand)); }
        }

        private bool CanExecuteNextCommand()
        {
            return CanUsePager && CurrentPageNumber < TotalPagesCount;
        }

        private void OnNextCommand()
        {
            var currentPageNumber = CurrentPageNumber;
            FirePagerStateChanged(++currentPageNumber);
        }

        private DelegateCommand _prevCommand;
        public DelegateCommand PrevCommand
        {
            get { return _prevCommand ?? (_prevCommand = new DelegateCommand(OnPrevCommand, CanExecutePrevCommand)); }
        }

        private bool CanExecutePrevCommand()
        {
            return CanUsePager && CurrentPageNumber > 1;
        }

        private void OnPrevCommand()
        {
            var currentPageNumber = CurrentPageNumber;
            FirePagerStateChanged(--currentPageNumber);
        }

        private DelegateCommand _firstCommand;
        public DelegateCommand FirstCommand
        {
            get { return _firstCommand ?? (_firstCommand = new DelegateCommand(OnFirstCommand, CanExecuteFirstCommand)); }
        }

        private bool CanExecuteFirstCommand()
        {
            return CanUsePager && CurrentPageNumber > 1;
        }

        private void OnFirstCommand()
        {
            FirePagerStateChanged(1);
        }

        private DelegateCommand _lastCommand;

        public DelegateCommand LastCommand
        {
            get { return _lastCommand ?? (_lastCommand = new DelegateCommand(OnLastCommand, CanExecuteLastCommand)); }
        }

        private bool CanExecuteLastCommand()
        {
            return CanUsePager && CurrentPageNumber < TotalPagesCount;
        }

        private void OnLastCommand()
        {
            FirePagerStateChanged(TotalPagesCount);
        }

        private bool CanUsePager
        {
            get { return _currentPageNumber > 0 && _totalPagesCount > 0 && _currentPageNumber <= _totalPagesCount; }
        }

        public ITitleProvider BeforePageText
        {
            get { return _beforePageText; }
        }

        public ITitleProvider AfterPageText
        {
            get { return _afterPageText; }
        }

        public ITitleProvider FirstPageText
        {
            get { return _firstPageText; }
        }

        public ITitleProvider LastPageText
        {
            get { return _lastPageText; }
        }

        public ITitleProvider NextPageText
        {
            get { return _nextPageText; }
        }

        public ITitleProvider PrevPageText
        {
            get { return _prevPageText; }
        }

        private void RaiseCommandsCanExecuteChanged()
        {
            NextCommand.RaiseCanExecuteChanged();
            PrevCommand.RaiseCanExecuteChanged();
            FirstCommand.RaiseCanExecuteChanged();
            LastCommand.RaiseCanExecuteChanged();
        }
    }
}
