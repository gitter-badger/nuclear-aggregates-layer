using System;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public abstract class UnityViewModelAspectResolverBase<TViewModelAspect, TViewModelFeature> : IViewModelAspectResolver
        where TViewModelAspect : class, IViewModelAspect
        where TViewModelFeature : class, IViewModelFeature 
    {
        private readonly Type _viewModelAspectType = typeof(TViewModelAspect);

        public bool TryResolveDependency(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity resolvingViewModelIdentity, out DependencyOverride resolvedDependency)
        {
            resolvedDependency = null;

            var aspectSpecificFeature = metadata.Features.OfType<TViewModelFeature>().SingleOrDefault();
            if (aspectSpecificFeature == null)
            {
                return false;
            }

            var viewModelAspect = Create(useCase, metadata, resolvingViewModelIdentity, aspectSpecificFeature);
            if (viewModelAspect == null)
            {
                return false;
            }

            resolvedDependency = new DependencyOverride(_viewModelAspectType, viewModelAspect);
            return true;
        }

        protected abstract TViewModelAspect Create(IUseCase useCase, IViewModelMetadata viewModelMetadata, IViewModelIdentity resolvingViewModelIdentity, TViewModelFeature feature);
    }
}