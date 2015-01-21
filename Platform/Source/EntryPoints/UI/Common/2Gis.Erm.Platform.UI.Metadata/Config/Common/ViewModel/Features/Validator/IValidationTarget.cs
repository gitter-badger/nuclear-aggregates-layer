using System.Collections.Generic;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public interface IValidationTarget : IMetadataElementAspect
    {
        bool ValidationEnabled { get; }
        IEnumerable<IValidatorViewModelFeature> Validators { get; }
    }
}
