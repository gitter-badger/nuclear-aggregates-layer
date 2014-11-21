namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual
{
    public sealed class ContextualDocumentContext : IContextualDocumentContext
    {
        public string Title { get; set; }
        public object Context { get; set; }
    }
}