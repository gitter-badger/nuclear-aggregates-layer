using System.Windows.Media;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure
{
    public interface IImageProvider
    {
        ImageSource Source { get; }
    }
}