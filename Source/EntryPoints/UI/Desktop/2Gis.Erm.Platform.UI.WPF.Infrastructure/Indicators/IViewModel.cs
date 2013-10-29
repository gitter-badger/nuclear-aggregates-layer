using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Indicators
{
    public interface IViewModel<TIdentity> : IViewModel
        where TIdentity : IViewModelIdentity
    {
        TIdentity ConcreteIdentity { get; }
    }
}
