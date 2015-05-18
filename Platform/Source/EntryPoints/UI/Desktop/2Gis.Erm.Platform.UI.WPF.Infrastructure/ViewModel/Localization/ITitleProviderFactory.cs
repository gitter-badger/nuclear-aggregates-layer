using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public interface ITitleProviderFactory
    {
        ITitleProvider Create(ITitleDescriptor descriptor);
    }
}
