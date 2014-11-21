using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public sealed class NullViewModel : ViewModelBase, IViewModel
    {
        private readonly OrdinaryViewModelIdentity _identity = new OrdinaryViewModelIdentity();

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        private object _context;
        public object Context
        {
            get
            {
                return _context;
            }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    RaisePropertyChanged(() => Context);
                }
            }
        }
    }
}
