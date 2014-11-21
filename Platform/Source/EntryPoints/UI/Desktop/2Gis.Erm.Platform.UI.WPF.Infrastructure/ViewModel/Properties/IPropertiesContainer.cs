using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties
{
    public interface IPropertiesContainer : IViewModelAspect
    {
        IEnumerable<IViewModelProperty> Properties { get; }
    }
}
