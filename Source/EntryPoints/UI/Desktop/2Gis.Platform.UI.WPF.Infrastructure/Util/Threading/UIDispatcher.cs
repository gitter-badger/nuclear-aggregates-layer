using System.Windows.Threading;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util.Threading
{
    public sealed class UIDispatcher : IUIDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public UIDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Dispatcher Current 
        {
            get
            {
                return _dispatcher;
            }
        }
    }
}