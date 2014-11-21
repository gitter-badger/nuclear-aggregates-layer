using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using FluentValidation;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public sealed class StaticValidatorViewModelFeature<TConcreteValidator, TViewModel> : IStaticValidatorViewModelFeature
        where TConcreteValidator : class, IValidator<TViewModel>, new()
        where TViewModel : class, IViewModel
    {
        public IValidator Create()
        {
            return new TConcreteValidator();
        }
    }
}