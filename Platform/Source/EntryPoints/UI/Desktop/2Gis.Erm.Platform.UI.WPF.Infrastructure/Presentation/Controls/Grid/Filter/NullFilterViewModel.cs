using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter
{
    public sealed class NullFilterViewModel : IFilterViewModel
    {
        public bool Enabled
        {
            get { return false; }
        }

        public event Action<string> ApplingFilter;
        public DelegateCommand FilterCommand { get; private set; }
        public string FilterText { get; set; }
        public ITitleProvider FilterTitle { get; private set; }
    }
}