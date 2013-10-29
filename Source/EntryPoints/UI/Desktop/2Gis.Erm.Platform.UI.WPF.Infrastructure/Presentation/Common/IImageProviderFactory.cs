using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common
{
    public interface IImageProviderFactory
    {
        IImageProvider Create(IImageDescriptor imageDescriptor);
    }
}
