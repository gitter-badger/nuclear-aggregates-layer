using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability
// ReSharper restore CheckNamespace
{
    public static partial class DesignTimeDataContainer
    {
        public static class Documents
        {
            public static ICompositeDocumentViewModel DefaultCardDocument
            {
                get
                {
                    return null;
                }
            }

            public static ContextualDocumentViewModel DefaultContextualDocumentViewModel
            {
                get
                {
                    return new ContextualDocumentViewModel(null);
                }
            }
        }
    }
}
