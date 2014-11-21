using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations
{
    public sealed class OperationItemViewModel : ViewModelBase
    {
        private readonly long _itemId;
        private IOperationResult _result;

        public OperationItemViewModel(long itemId)
        {
            _itemId = itemId;
        }

        public long ItemId
        {
            get { return _itemId; }
        }

        public IOperationResult Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}