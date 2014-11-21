namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual
{
    /// <summary>
    /// Особый вид документа - его содержимое оперделяется выбором пользователя в navigation панели (исключая, контекстную область панели навигации)
    /// </summary>
    public interface IContextualDocument : IDocument, IActivableDocument
    {
        IContextualDocumentContext Context { get; set; }
    }
}
