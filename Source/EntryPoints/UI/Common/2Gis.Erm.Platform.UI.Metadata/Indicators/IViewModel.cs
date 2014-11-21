namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    /// <summary>
    /// Базовый интерфейс viewmodel
    /// </summary>
    public interface IViewModel : IViewModelBase
    {
        IViewModelIdentity Identity { get; }
    }
}
