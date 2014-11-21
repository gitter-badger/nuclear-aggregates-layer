using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties
{
    public sealed class PropertiesContainer : IPropertiesContainer
    {
        private readonly IEnumerable<IViewModelProperty> _properties;

        public PropertiesContainer(IEnumerable<IViewModelProperty> properties)
        {
            _properties = properties;
        }

        public IEnumerable<IViewModelProperty> Properties
        {
            get
            {
                return _properties;
            }
        }

        bool IViewModelAspect.Enabled
        {
            get
            {
                return true;
            }
        }
    }
}