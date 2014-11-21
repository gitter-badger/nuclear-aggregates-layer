using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public sealed class CompositeValidatorViewModelFeature : IValidatorViewModelFeature
    {
        private readonly List<IValidatorViewModelFeature> _validators = new List<IValidatorViewModelFeature>();

        public ICollection<IValidatorViewModelFeature> Validators
        {
            get
            {
                return _validators;
            }
        }
    }
}