using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;

using FluentValidation;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public sealed class UnityViewModelValidatorsResolver : UnityViewModelAspectResolverBase<IValidatorsContainer, IValidatorViewModelFeature>
    {
        private readonly IMetadata2ViewModelPropertiesMapper[] _propertiesMappers;

        private delegate bool ValidatorFeatureProcessor(
            IUseCase useCase,
            IViewModelStructure viewModelStructure,
            IViewModelIdentity resolvingViewModelIdentity,
            IValidatorViewModelFeature feature,
            out IValidator validator);

        private readonly IEnumerable<ValidatorFeatureProcessor> _featureProcessors;

        // ReSharper disable ParameterTypeCanBeEnumerable.Local
        public UnityViewModelValidatorsResolver(IMetadata2ViewModelPropertiesMapper[] propertiesMappers)
        // ReSharper restore ParameterTypeCanBeEnumerable.Local
        {
            _propertiesMappers = propertiesMappers;
            _featureProcessors = new ValidatorFeatureProcessor[] { TryProcessDynamicValidatorFeature, TryProcessStaticValidatorFeature };
        }

        protected override IValidatorsContainer Create(IUseCase useCase, IViewModelStructure viewModelStructure, IViewModelIdentity resolvingViewModelIdentity, IValidatorViewModelFeature feature)
        {
            var validatorsFeature = feature as CompositeValidatorViewModelFeature;
            if (validatorsFeature == null || validatorsFeature.Validators == null)
            {
                return null;
            }

            var validators = new List<IValidator>();

            foreach (var validatorFeature in validatorsFeature.Validators)
            {
                foreach (var processor in _featureProcessors)
                {
                    IValidator validator;
                    if (processor(useCase, viewModelStructure, resolvingViewModelIdentity, validatorFeature, out validator))
                    {
                        validators.Add(validator);
                        break;
                    }
                }
            }

            var factory = useCase.ResolveFactoryContext();
            return factory.Resolve<ValidationContainer>(new DependencyOverrides { { typeof(IEnumerable<IValidator>), validators } });
        }

        private bool TryProcessDynamicValidatorFeature(
            IUseCase useCase, 
            IViewModelStructure viewModelStructure,
            IViewModelIdentity resolvingViewModelIdentity,
            IValidatorViewModelFeature feature, 
            out IValidator validator)
        {
            validator = null;
            var dynamicValidatorFeature = feature as IDynamicValidatorModelFeature;
            if (dynamicValidatorFeature == null)
            {
                return false;
            }

            var resolvedViewModelProperties = _propertiesMappers.ResolveViewModelProperties(useCase, viewModelStructure, resolvingViewModelIdentity);

            var proxyType = typeof(ProxyValidator<,>).MakeGenericType(dynamicValidatorFeature.ConcreteValidatorType, dynamicValidatorFeature.ViewModelType);
            validator = //(IValidator)Activator.CreateInstance(proxyType, resolvedViewModelProperties);
                proxyType.New<IValidator>(resolvedViewModelProperties);
            return true;
        }

        private bool TryProcessStaticValidatorFeature(
            IUseCase useCase,
            IViewModelStructure viewModelStructure,
            IViewModelIdentity resolvingViewModelIdentity,
            IValidatorViewModelFeature feature,
            out IValidator validator)
        {
            validator = null;
            var staticValidatorFeature = feature as IStaticValidatorViewModelFeature;
            if (staticValidatorFeature == null)
            {
                return false;
            }

            validator = staticValidatorFeature.Create();

            return true;
        }
    }
}