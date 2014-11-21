using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface INotificationsProcessor : ISimplifiedModelConsumer
    {
        void Process();
    }
}
