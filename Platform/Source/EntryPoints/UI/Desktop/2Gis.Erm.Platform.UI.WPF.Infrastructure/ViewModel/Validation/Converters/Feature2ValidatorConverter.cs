using System;

using FluentValidation.Validators;

using NuClear.Metamodeling.Domain.Entities;

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

        protected TValidatablePropertyFeature Settings { get; set; }

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

        protected abstract IPropertyValidator CreateValidator();
        protected abstract ErrorDescription GetErrorDescription();
    }
}