using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using FluentValidation;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public sealed class ValidatorViewModelFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IValidationTarget, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IValidationTarget
    {
        public ValidatorViewModelFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Static<TConcreteValidator, TViewModel>()
            where TConcreteValidator : class, IValidator<TViewModel>, new()
            where TViewModel : class, IViewModel
        {
            AddValidator(new StaticValidatorViewModelFeature<TConcreteValidator, TViewModel>());
            return AspectHostBuilder;
        }

        public TBuilder Dynamic<TConcreteValidator, TViewModel>()
            where TConcreteValidator : IDynamicViewModelValidator<TViewModel>, new()
            where TViewModel : class, IDynamicViewModel
        {
            AddValidator(new DynamicValidatorModelFeature<TConcreteValidator, TViewModel>());
            return AspectHostBuilder;
        }

        private void AddValidator<TValidatorFeature>(TValidatorFeature validatorFeature)
            where TValidatorFeature : class, IValidatorViewModelFeature, new()
        {
            var compositeValidatorFeature = AspectHostBuilder.Features.OfType<CompositeValidatorViewModelFeature>().SingleOrDefault();
            if (compositeValidatorFeature == null)
            {
                compositeValidatorFeature = new CompositeValidatorViewModelFeature();
                AspectHostBuilder.Features.Add(compositeValidatorFeature);
            }

            if (validatorFeature is IDynamicValidatorModelFeature && compositeValidatorFeature.Validators.Any(feature => feature is IDynamicValidatorModelFeature))
            {
                throw new InvalidOperationException("Only one " + typeof(IDynamicValidatorModelFeature).Name + " feature must be specified");
            }

            compositeValidatorFeature.Validators.Add(validatorFeature);
        }
    }
}
