using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.Views
{
    public static class ImageSourceContainer
    {
        private static readonly Uri ErrorIconUri = new Uri("pack://application:,,,/2Gis.Erm.Platform.UI.WPF.Infrastructure;component/Presentation/Resources/Images/error.png");
        private static readonly Uri WarningIconUri = new Uri("pack://application:,,,/2Gis.Erm.Platform.UI.WPF.Infrastructure;component/Presentation/Resources/Images/warning.png");
        private static readonly Uri MessageIconUri = new Uri("pack://application:,,,/2Gis.Erm.Platform.UI.WPF.Infrastructure;component/Presentation/Resources/Images/information.png");

        public static ImageSource ErrorIcon
        {
            get { return new CachedBitmap(new BitmapImage(ErrorIconUri), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand); }
        }

        public static ImageSource WarningIcon
        {
            get { return new CachedBitmap(new BitmapImage(WarningIconUri), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand); }
        }

        public static ImageSource MessageIcon
        {
            get { return new CachedBitmap(new BitmapImage(MessageIconUri), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand); }
        }
    }
}