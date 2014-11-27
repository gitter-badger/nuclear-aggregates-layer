namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    /// <summary>
    /// Базовый интерфейс viewmodel
    /// </summary>
    public interface IViewModel : IViewModelAbstract
    {
        IViewModelIdentity Identity { get; }
    }
}