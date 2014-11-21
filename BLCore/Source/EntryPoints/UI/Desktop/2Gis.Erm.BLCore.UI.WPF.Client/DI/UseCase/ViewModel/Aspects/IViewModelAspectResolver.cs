using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public interface IViewModelAspectResolver
    {
        bool TryResolveDependency(
            IUseCase useCase,
            IViewModelMetadata metadata,
            IViewModelIdentity resolvingViewModelIdentity,
            out DependencyOverride resolvedDependency);
    }
}
