using System.ComponentModel;
using System.Windows;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util
{
    public static class DesignTimeUtils
    {
        public static bool IsDesignMode 
        {
            get
            {
                return
                    (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }
    }
}
