using FluentValidation;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public interface IStaticValidatorViewModelFeature : IValidatorViewModelFeature
    {
        IValidator Create();
    }
}