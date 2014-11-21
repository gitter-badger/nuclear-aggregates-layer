using System.Collections.Generic;
using System.Reflection;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using FluentValidation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public interface IInitializableDynamicViewModelValidator : IValidator
    {
        void Initialize(ICustomTypeProvider validationTargetDynamicType, IEnumerable<IViewModelProperty> dynamicPropertiesDescriptions);
    }
}
