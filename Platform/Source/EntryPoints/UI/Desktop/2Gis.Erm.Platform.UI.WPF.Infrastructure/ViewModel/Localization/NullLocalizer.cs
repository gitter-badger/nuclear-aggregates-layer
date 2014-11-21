using System.Globalization;

using DoubleGis.Platform.UI.WPF.Infrastructure.Localization;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public sealed class NullLocalizer : ILocalizer
    {
        private readonly DynamicResourceDictionary _stubResourceDictionary = new DynamicResourceDictionary(CultureInfo.CurrentUICulture);
        public DynamicResourceDictionary Localized
        {
            get
            {
                return _stubResourceDictionary;
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