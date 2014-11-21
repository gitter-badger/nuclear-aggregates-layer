using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties
{
    public sealed class NullPropertiesContainer : IPropertiesContainer
    {
        public IEnumerable<IViewModelProperty> Properties
        {
            get
            {
                return new IViewModelProperty[0];
            }
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