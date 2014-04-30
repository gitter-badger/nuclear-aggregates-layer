using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Validator
{
    public interface IValidationTarget : IMetadataElementAspect
    {
        bool ValidationEnabled { get; }
        IEnumerable<IValidatorViewModelFeature> Validators { get; }
    }
}
