using System;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using FluentValidation;
using FluentValidation.Internal;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public abstract class AbstractDynamicViewModelValidator<TViewModel> : AbstractValidator<TViewModel>, IDynamicViewModelValidator<TViewModel> 
        where TViewModel : class, IDynamicViewModel
    {
        private Type _customType;

        protected void InitializeDynamicViewModelType(ICustomTypeProvider dynamicViewModelTypeProvider)
        {
            _customType = dynamicViewModelTypeProvider.GetCustomType();
        }

        private IRuleBuilderInitial<TViewModel, TProperty> RuleFor<TProperty>(string propertyName)
        {
            if (_customType == null)
            {
                throw new InvalidOperationException("Before rule configuring initialization method must be called");
            }

            Func<object, object> propertyValueFunc = vm => ((IDynamicPropertiesContainer)vm).GetDynamicPropertyValue(propertyName);
            var getMethod = _customType.GetProperty(propertyName).GetGetMethod();

            var rule = new PropertyRule(getMethod,
                propertyValueFunc,
                null,
                () => ValidatorOptions.CascadeMode,
                typeof(TProperty),
                typeof(TViewModel)) { PropertyName = propertyName };
 
            AddRule(rule);
            return new RuleBuilder<TViewModel, TProperty>(rule);
        }

        protected IRuleBuilderInitial<TViewModel, TProperty> RuleForDtoProperty<TDto, TProperty>(Expression<Func<TDto, TProperty>> expression)
        {
            return RuleFor<TProperty>(StaticReflection.GetMemberName(expression));
        }

        protected IRuleBuilderInitial<TViewModel, object> RuleForProperty(string propertyName)
        {
            return RuleFor<object>(propertyName);
        }

        protected IRuleBuilderInitial<TViewModel, TProperty> RuleForProperty<TProperty>(string propertyName)
        {
            return RuleFor<TProperty>(propertyName);
        }
    }
}