using System;

namespace DoubleGis.Platform.UI.WPF.Resources
{
    public static class DictionaryUrls
    {
        private static readonly Uri ModernUIUri = new Uri("pack://application:,,,/FirstFloor.ModernUI;component/Assets/ModernUI.xaml");
        private static readonly Uri ModernUILightUri = new Uri("pack://application:,,,/FirstFloor.ModernUI;component/Assets/ModernUI.Light.xaml");
        private static readonly Uri ModernButtonUri = new Uri("pack://application:,,,/FirstFloor.ModernUI;component/Assets/Button.xaml");
        private static readonly Uri ModernConvertersUri = new Uri("pack://application:,,,/FirstFloor.ModernUI;component/Assets/Converters.xaml");
        private static readonly Uri ModernWindowUri = new Uri("pack://application:,,,/2Gis.Platform.UI.WPF.Resources;component/Styles/ModernWindow.xaml");
        private static readonly Uri MenuUri = new Uri("pack://application:,,,/2Gis.Platform.UI.WPF.Resources;component/Styles/Menu.xaml");

        public static Uri ModernUI
        {
            get { return ModernUIUri; }
        }

        public static Uri ModernUILight
        {
            get { return ModernUILightUri; }
        }

        public static Uri ModernButton
        {
            get { return ModernButtonUri; }
        }

        public static Uri ModernConverters
        {
            get { return ModernConvertersUri; }
        }

        public static Uri ModernWindow
        {
            get { return ModernWindowUri; }
        }

        public static Uri Menu
        {
            get { return MenuUri; }
        }
    }
}
