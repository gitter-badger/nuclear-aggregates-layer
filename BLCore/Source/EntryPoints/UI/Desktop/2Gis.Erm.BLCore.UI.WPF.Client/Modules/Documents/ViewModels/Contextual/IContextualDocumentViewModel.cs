using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents.Contextual;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual
{
    public interface IContextualDocumentViewModel : ICompositeViewModel, IContextualDocument
    {
        DataTemplateSelector ViewSelector { get; }
    }
}