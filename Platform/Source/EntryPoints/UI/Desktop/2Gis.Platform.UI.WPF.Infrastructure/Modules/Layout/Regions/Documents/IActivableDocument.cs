namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents
{
    /// <summary>
    /// Документ поддерживающий возможность активации/декативации, а, следовательно, и специфическую реакцию на смену состояния активности
    /// </summary>
    public interface IActivableDocument
    {
        bool IsActive { get; set; }
    }
}
