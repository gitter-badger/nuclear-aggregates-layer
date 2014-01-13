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

        public bool TryResolveDependency(IUseCase useCase, IViewModelStructure structure, IViewModelIdentity resolvingViewModelIdentity, out DependencyOverride resolvedDependency)
        {
            resolvedDependency = null;

            var aspectSpecificFeature = structure.ElementFeatures.OfType<TViewModelFeature>().SingleOrDefault();
            if (aspectSpecificFeature == null)
            {
                return false;
            }

            var viewModelAspect = Create(useCase, structure, resolvingViewModelIdentity, aspectSpecificFeature);
            if (viewModelAspect == null)
            {
                return false;
            }

            resolvedDependency = new DependencyOverride(_viewModelAspectType, viewModelAspect);
            return true;
        }

        protected abstract TViewModelAspect Create(IUseCase useCase, IViewModelStructure viewModelStructure, IViewModelIdentity resolvingViewModelIdentity, TViewModelFeature feature);
    }
}