using System;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public interface IDynamicValidatorModelFeature : IValidatorViewModelFeature
    {
        Type ConcreteValidatorType { get; }
        Type ViewModelType { get; }
    }
}