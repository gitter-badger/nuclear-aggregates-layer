using System;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelTitleResolver : IViewModelAspectResolver
    {
        private readonly ITitleProviderFactory _titleProviderFactory; 

        public UnityViewModelTitleResolver(ITitleProviderFactory titleProviderFactory)
        {
            _titleProviderFactory = titleProviderFactory;
        }

        private readonly Type _viewModelAspectType = typeof(ITitleProvider);

        public bool TryResolveDependency(IUseCase useCase, IViewModelStructure structure, IViewModelIdentity resolvingViewModelIdentity, out DependencyOverride resolvedDependency)
        {
            var titleProvider = _titleProviderFactory.Create(structure.TitleDescriptor);
            resolvedDependency = new DependencyOverride(_viewModelAspectType, titleProvider);
            return true;
        }
    }
}
