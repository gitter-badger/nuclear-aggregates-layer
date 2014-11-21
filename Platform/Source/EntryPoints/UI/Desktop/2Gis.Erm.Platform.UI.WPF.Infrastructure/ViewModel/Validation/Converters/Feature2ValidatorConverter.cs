using System;

using DoubleGis.Erm.Platform.Model.Metadata.Entities;

using FluentValidation.Validators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public abstract class Feature2ValidatorConverter<TConcreteConverter, TValidatablePropertyFeature>
        where TValidatablePropertyFeature : class, IValidatablePropertyFeature
        where TConcreteConverter : Feature2ValidatorConverter<TConcreteConverter, TValidatablePropertyFeature>, new()
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly Type TargetValidatorFeature = typeof(TValidatablePropertyFeature);
        // ReSharper restore StaticFieldInGenericType
        
        public static Type TargetFeature
        {
            get { return TargetValidatorFeature; }
        }

        public static bool TryConvert(IValidatablePropertyFeature validatablePropertyFeature, out IPropertyValidator validator)
        {
            validator = null;

            var targetFeature = validatablePropertyFeature as TValidatablePropertyFeature;
            if (targetFeature == null)
            {
                return false;
            }

            var concreteContext = new TConcreteConverter { Settings = targetFeature };
            validator = concreteContext.CreateValidator();
            validator.CustomStateProvider = o => concreteContext.GetErrorDescription();
            
            return true;
        }

        protected TValidatablePropertyFeature Settings { get; set; }

        protected abstract IPropertyValidator CreateValidator();
        protected abstract ErrorDescription GetErrorDescription();
    }
}