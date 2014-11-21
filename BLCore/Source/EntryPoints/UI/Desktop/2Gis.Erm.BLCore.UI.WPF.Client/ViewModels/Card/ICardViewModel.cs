using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardViewModel : IViewModel, IDocument
    {
    }

    public interface ICardViewModel<TCardViewModelIdentity> : IViewModel<TCardViewModelIdentity>, ICardViewModel
        where TCardViewModelIdentity : class, ICardViewModelIdentity
    {
    }
}
