using System;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelLocalizerResolver : UnityViewModelAspectResolverBase<ILocalizer, LocalizeViewModelFeature>
    {
        protected override ILocalizer Create(IUseCase useCase, IViewModelStructure viewModelStructure, IViewModelIdentity resolvingViewModelIdentity, LocalizeViewModelFeature feature)
        {
            var factory = useCase.ResolveFactoryContext();
            return factory.Resolve<Localizer>(new DependencyOverrides { { typeof(Type[]), feature.ResourceManagerHostTypes } });
        }
    }
}