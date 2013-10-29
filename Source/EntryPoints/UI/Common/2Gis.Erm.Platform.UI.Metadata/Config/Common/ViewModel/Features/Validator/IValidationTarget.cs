using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public interface IValidationTarget : IConfigElementAspect
    {
        bool ValidationEnabled { get; }
        IEnumerable<IValidatorViewModelFeature> Validators { get; }
    }
}
