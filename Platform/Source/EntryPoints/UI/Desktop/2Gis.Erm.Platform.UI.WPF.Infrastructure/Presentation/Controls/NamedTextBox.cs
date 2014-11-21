using System.Windows;
using System.Windows.Controls;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls
{
    public class NamedTextBox : TextBox
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(NamedTextBox), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PropertyNameRegionWidthProperty = DependencyProperty.Register("PropertyNameRegionWidth", typeof(GridLength), typeof(NamedTextBox), new PropertyMetadata(default(GridLength)));

        static NamedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NamedTextBox), new FrameworkPropertyMetadata(typeof(NamedTextBox)));
            IsTabStopProperty.OverrideMetadata(typeof(NamedTextBox), new FrameworkPropertyMetadata(false));
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