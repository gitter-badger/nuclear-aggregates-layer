using System.Windows.Threading;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Threading
{
    public interface IUIDispatcher
    {
        Dispatcher Current { get; }
    }
}
