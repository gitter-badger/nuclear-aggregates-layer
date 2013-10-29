using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public sealed class DynamicValidatorModelFeature<TConcreteValidator, TViewModel> : IDynamicValidatorModelFeature 
        where TConcreteValidator : IDynamicViewModelValidator<TViewModel>, new()
        where TViewModel : class, IDynamicViewModel
    {
        private readonly Type _concreteValidatorType = typeof(TConcreteValidator);
        private readonly Type _viewModelType = typeof(TViewModel);

        public Type ConcreteValidatorType
        {
            get { return _concreteValidatorType; }
        }

        public Type ViewModelType
        {
            get { return _viewModelType; }
        }
    }
}