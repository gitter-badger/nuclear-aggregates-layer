using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using FluentValidation.Results;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public sealed class NullValidatorsContainer : IValidatorsContainer
    {
        private readonly IDictionary<string, ICollection<string>> _errors = new Dictionary<string, ICollection<string>>();
        private readonly IEnumerable<string> _changedErrors = Enumerable.Empty<string>();

        public IEnumerable<string> ChangedErrors
        {
            get { return _changedErrors; }
        }

        public ValidationResult Validate(IViewModel viewModel)
        {
            return new ValidationResult();
        }

        public IDictionary<string, ICollection<string>> Errors
        {
            get { return _errors; }
        }

        bool IViewModelAspect.Enabled
        {
            get
            {
                return false;
            }
        }
    }
}