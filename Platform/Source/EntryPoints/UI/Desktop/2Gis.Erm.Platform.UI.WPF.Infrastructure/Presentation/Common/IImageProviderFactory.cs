using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common
{
    public interface IImageProviderFactory
    {
        IImageProvider Create(IImageDescriptor imageDescriptor);
    }
}
