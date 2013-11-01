using System;

namespace DoubleGis.Platform.UI.WPF.Resources
{
    public static class ResourceUrls
    {
        private static readonly Uri LogoUri = new Uri("pack://application:,,,/2Gis.Platform.UI.WPF.Resources;component/Images/2GisLogo.png");

        public static Uri Logo
        {
            get { return LogoUri; }
        }
    }
}