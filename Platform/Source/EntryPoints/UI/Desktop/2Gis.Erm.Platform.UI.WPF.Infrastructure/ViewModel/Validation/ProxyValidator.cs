using System;
using System.Collections.Generic;
using System.Reflection;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using FluentValidation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public class ProxyValidator<TConcreteValidator, TViewModel> : AbstractValidator<TViewModel>
        where TConcreteValidator : DynamicViewModelValidator<TViewModel>, new()
        where TViewModel : class, ICustomTypeProvider, IDynamicPropertiesContainer, IDynamicViewModel
    {
        private readonly IEnumerable<IViewModelProperty> _viewModelProperties;
        private readonly Lazy<IInitializableDynamicViewModelValidator> _wrappedValidator;

        public ProxyValidator(IEnumerable<IViewModelProperty> viewModelProperties)
        {
            _viewModelProperties = viewModelProperties;
            _wrappedValidator = new Lazy<IInitializableDynamicViewModelValidator>(() => new TConcreteValidator());
        }

        public override FluentValidation.Results.ValidationResult Validate(TViewModel instance)
        {
            if (!_wrappedValidator.IsValueCreated)
            {
                _wrappedValidator.Value.Initialize(instance, _viewModelProperties);
            }

            return _wrappedValidator.Value.Validate(instance);
        }
    }
}
