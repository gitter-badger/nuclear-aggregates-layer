using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager
{
    public sealed class NullPagerViewModel : IPagerViewModel
    {
        public bool Enabled 
        {
            get { return false; }
        }

        public event Action<int> TryChangingTargetPage;
        public void UpdatePager(int currentPageNumber, int totalPagesCount)
        {
            throw new NotImplementedException();
        }

        public int TotalPagesCount { get; private set; }
        public int CurrentPageNumber { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PrevCommand { get; private set; }
        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public ITitleProvider BeforePageText { get; private set; }
        public ITitleProvider AfterPageText { get; private set; }
        public ITitleProvider FirstPageText { get; private set; }
        public ITitleProvider LastPageText { get; private set; }
        public ITitleProvider NextPageText { get; private set; }
        public ITitleProvider PrevPageText { get; private set; }
    }
}