namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    /// <summary>
    /// Маркерный интерфейс для динамических viewmodels - динамическими их делает наличие свойств доступных для binding, но при этом отсутствующих в 
    /// статических метаданных конкретного типа viewmodel
    /// </summary>
    public interface IDynamicViewModel : IViewModel
    {
    }
}