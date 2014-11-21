using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls
{
    public class NamedDatePicker : DatePicker
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedDatePicker), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedDatePicker), new PropertyMetadata(default(GridLength)));

        static NamedDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NamedDatePicker), new FrameworkPropertyMetadata(typeof(NamedDatePicker)));
            IsTabStopProperty.OverrideMetadata(typeof(NamedDatePicker), new FrameworkPropertyMetadata(false));
        }

        public string PropertyName
        {
            get
            {
                return (string)GetValue(PropertyNameProperty);
            }

            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        public GridLength PropertyNameRegionWidth
        {
            get
            {
                return (GridLength)GetValue(PropertyNameRegionWidthProperty);
            }

            set
            {
                SetValue(PropertyNameRegionWidthProperty, value);
            }
        } 
    }
}