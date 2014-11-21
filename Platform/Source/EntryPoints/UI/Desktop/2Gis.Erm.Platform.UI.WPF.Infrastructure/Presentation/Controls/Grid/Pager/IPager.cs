using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager
{
    public interface IPager
    {
        event Action<int> TryChangingTargetPage;

        void UpdatePager(int currentPageNumber, int totalPagesCount);

        int TotalPagesCount { get; }
        int CurrentPageNumber { get; }

        DelegateCommand NextCommand { get; }
        DelegateCommand PrevCommand { get; }
        DelegateCommand FirstCommand { get; }
        DelegateCommand LastCommand { get; }

        ITitleProvider BeforePageText { get; }
        ITitleProvider AfterPageText { get; }
        ITitleProvider FirstPageText { get; }
        ITitleProvider LastPageText { get; }
        ITitleProvider NextPageText { get; }
        ITitleProvider PrevPageText { get; }
    }
}