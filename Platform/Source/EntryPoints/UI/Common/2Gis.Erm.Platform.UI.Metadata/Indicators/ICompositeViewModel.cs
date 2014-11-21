namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    /// <summary>
    /// Маркреный интерфейс viewmodel клиента
    /// </summary>
    public interface ICompositeViewModel : IViewModel
    {
        IViewModel[] ComposedViewModels { get; }
    }
}