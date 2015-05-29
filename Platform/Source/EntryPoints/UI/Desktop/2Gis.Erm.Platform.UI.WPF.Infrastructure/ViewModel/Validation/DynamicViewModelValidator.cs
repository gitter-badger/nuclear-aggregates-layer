using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public class DynamicViewModelValidator<TViewModel> : AbstractDynamicViewModelValidator<TViewModel>, IInitializableDynamicViewModelValidator 
        where TViewModel : class, IDynamicViewModel
    {
        public void Initialize(ICustomTypeProvider validationTargetDynamicType, IEnumerable<IViewModelProperty> dynamicPropertiesDescriptions)
        {
            InitializeDynamicViewModelType(validationTargetDynamicType);

            ProvideRules();

            foreach (var prop in dynamicPropertiesDescriptions)
            {
                foreach (var validator in ValidatorFeatureToValidatorsConverter.Convert(prop.Features.OfType<IValidatablePropertyFeature>()))
                {
                    RuleForProperty(prop.Name).SetValidator(validator);
                }
            }
        }

        protected virtual void ProvideRules()
        {
        }
    }
}