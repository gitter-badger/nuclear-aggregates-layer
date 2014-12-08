using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public interface ICardMainAttributeFeature : ICardFeature
    {
        string PropertyName { get; }
        bool TryExecute(IViewModelAbstract viewModel, out object result);
    }
}