using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelPropertiesResolver : UnityViewModelAspectResolverBase<IPropertiesContainer, DynamicPropertiesFeature>
    {
        private readonly IMetadata2ViewModelPropertiesMapper[] _propertiesMappers;

        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        public UnityViewModelPropertiesResolver(IMetadata2ViewModelPropertiesMapper[] propertiesMappers)
        // ReSharper restore ParameterTypeCanBeEnumerable.Local
        {
            _propertiesMappers = propertiesMappers;
        }

        protected override IPropertiesContainer Create(IUseCase useCase, IViewModelStructure viewModelStructure, IViewModelIdentity resolvingViewModelIdentity, DynamicPropertiesFeature feature)
        {
            var viewModelProperties = _propertiesMappers.ResolveViewModelProperties(useCase, viewModelStructure, resolvingViewModelIdentity);
            return new PropertiesContainer(viewModelProperties);
        }
    }
}