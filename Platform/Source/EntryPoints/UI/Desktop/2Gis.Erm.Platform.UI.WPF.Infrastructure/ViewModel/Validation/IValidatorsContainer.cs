using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using FluentValidation.Results;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public interface IValidatorsContainer : IViewModelAspect
    {
        IDictionary<string, ICollection<string>> Errors { get; }
        IEnumerable<string> ChangedErrors { get; }
        ValidationResult Validate(IViewModel viewModel);
    }
}
