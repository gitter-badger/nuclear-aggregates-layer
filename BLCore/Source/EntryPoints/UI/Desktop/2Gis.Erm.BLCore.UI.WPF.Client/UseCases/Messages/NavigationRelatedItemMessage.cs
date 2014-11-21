using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages
{
    public sealed class NavigationRelatedItemMessage : MessageBase<SequentialProcessingModel>
    {
        private readonly INavigationItem _usedItem;

        public NavigationRelatedItemMessage(INavigationItem usedItem)
            :base(null)
        {
            _usedItem = usedItem;
        }

        public INavigationItem UsedItem
        {
            get
            {
                return _usedItem;
            }
        }
    }
}
